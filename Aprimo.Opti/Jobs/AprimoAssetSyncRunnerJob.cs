using Aprimo.Opti.Models.RestImpl.Search;
using Aprimo.Opti.Services;
using EPiServer;
using EPiServer.DataAbstraction;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aprimo.Opti.Jobs
{
    [ScheduledPlugIn(DisplayName = "Aprimo Asset Sync Runner", SortIndex = 8001, GUID = "F54ABFCE-A92E-48B9-BDE8-DD2291CD6BCD", IntervalLength = 1, IntervalType = ScheduledIntervalType.Minutes)]
    public class AprimoAssetSyncRunnerJob : ScheduledJobBase
    {
        private bool _stopSignaled;

        private readonly IAprimoAssetPersistantService aprimoAssetPersistantService;

        private readonly IdentityMappingService identityMappingService;

        private readonly IAprimoAssetService aprimoAssetService;

        private readonly IContentCacheRemover contentCacheRemover;

        public AprimoAssetSyncRunnerJob(
            IAprimoAssetPersistantService aprimoAssetPersistantService,
            IdentityMappingService identityMappingService,
            IAprimoAssetService aprimoAssetService,
            IContentCacheRemover contentCacheRemover)
        {
            IsStoppable = true;
            this.aprimoAssetPersistantService = aprimoAssetPersistantService;
            this.identityMappingService = identityMappingService;
            this.aprimoAssetService = aprimoAssetService;
            this.contentCacheRemover = contentCacheRemover;
        }

        /// <summary>
        /// Called when a user clicks on Stop for a manually started job, or when ASP.NET shuts down.
        /// </summary>
        public override void Stop()
        {
            _stopSignaled = true;
        }

        /// <summary>
        /// Called when a scheduled job executes
        /// </summary>
        /// <returns>A status message to be stored in the database log and visible from admin mode</returns>
        public override string Execute()
        {
            int current = 0;

            OnStatusChanged($"Starting execution of {this.GetType()}");

            // Var get assets by dateRange
            var request = new SearchRequest
            {
                PageSize = 100,
                SearchExpression = new SearchExpression()
                {
                    Expression = $"ModifiedOn > '{DateTime.UtcNow.AddMinutes(-1)}'"
                }
            };

            var assets = this.aprimoAssetService.SearchAsset(request);

            // Get all get all assets from mapping table

            foreach (var fileAsset in assets.Items)
            {
                var persistantAssets = this.aprimoAssetPersistantService.GetAssets(new Guid(fileAsset.Id));
                if (persistantAssets.Any())
                {
                    foreach (var persistantAsset in persistantAssets)
                    {
                        persistantAsset.ModifiedDate = fileAsset.ModifiedOn.Date;
                        if (!string.IsNullOrWhiteSpace(fileAsset?.Embedded?.MasterFileVersion?.FileExtension))
                        {
                            persistantAsset.Extension = fileAsset?.Embedded?.MasterFileVersion?.FileExtension;
                        }

                        // Save the fields that we have mapped.  no need for other fields since this is all the model relys on to fill
                        // the values of the contentitem
                        if (fileAsset.Embedded.Fields.Items.Any())
                        {
                            List<KeyValuePair<string, string>> fields = new List<KeyValuePair<string, string>>();
                            foreach (var item in fileAsset.Embedded.Fields.Items)
                            {
                                fields.Add(new KeyValuePair<string, string>(item.FieldName, fileAsset.GetFieldValue(item.FieldName)));
                            }
                            persistantAsset.MetaInformation = JsonConvert.SerializeObject(fields);
                        }

                        this.aprimoAssetPersistantService.Update(persistantAsset);
                        current++;

                        if (_stopSignaled)
                        {
                            return "Stop of job was called";
                        }
                    }
                }
                this.OnStatusChanged($"Processed {current} items");

                if (_stopSignaled)
                {
                    return "Stop of job was called";
                }
            }

            return $"Processed {current} items";
        }
    }
}