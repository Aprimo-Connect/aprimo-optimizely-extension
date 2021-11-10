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
    [ScheduledPlugIn(DisplayName = "Aprimo Sync All Assets Job", SortIndex = 8000, GUID = "ADD95854-B054-4FE4-A972-8E4A3CF163C2")]
    public class AprimoSyncJob : ScheduledJobBase
    {
        private bool _stopSignaled;

        private readonly IAprimoAssetPersistantService aprimoAssetPersistantService;

        private readonly IdentityMappingService identityMappingService;

        private readonly IAprimoAssetService aprimoAssetService;

        private readonly IContentCacheRemover contentCacheRemover;

        public AprimoSyncJob(
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
            int total = 0;
            int current = 0;

            OnStatusChanged($"Starting execution of {this.GetType()}");

            // Get all get all assets from mapping table
            var allAssets = this.identityMappingService.List(AprimoConstants.ProviderKey);
            total = allAssets.Count();

            foreach (var mappedIdentity in allAssets)
            {
                var renditionId = mappedIdentity.ExternalIdentifier.Segments.Last();
                var masterId = mappedIdentity.ExternalIdentifier.Segments.ElementAt(1).Trim(new char[] { '/' });

                var fileAsset = this.aprimoAssetService.GetAsset(masterId);
                if (fileAsset != null)
                {
                    var persistantAsset = this.aprimoAssetPersistantService.GetAsset(new Guid(masterId), renditionId);
                    if (persistantAsset != null)
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
                    }
                    current++;
                    this.OnStatusChanged($"Processed {current} of {total}");

                    contentCacheRemover.Remove(mappedIdentity.ContentLink);
                }
                //For long running jobs periodically check if stop is signaled and if so stop execution
                if (_stopSignaled)
                {
                    return "Stop of job was called";
                }
            }

            this.contentCacheRemover.Remove(allAssets.Select(x => x.ContentLink));

            this.contentCacheRemover.Remove(AprimoContentProvider.GetRoot());

            return $"Processed {current} of {total}";
        }
    }
}