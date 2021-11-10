using Aprimo.Opti.AprimoPersistance;
using Aprimo.Opti.Models;
using Aprimo.Opti.Services;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Shell.Services.Rest;
using EPiServer.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aprimo.Opti
{
    [RestStore("aprimoassetmappingstore")]
    public class AprimoAssetMappingStore : RestControllerBase
    {
        private readonly IdentityMappingService identityMappingService;

        private readonly IContentLoader contentLoader;

        private readonly IAprimoAssetPersistantService aprimoAssetPersistantService;

        private readonly IAprimoAssetService aprimoAssetService;

        private readonly IContentCacheRemover contentCacheRemover;

        private readonly AprimoContentModelStore aprimoContentModelStore;

        public AprimoAssetMappingStore(
            IdentityMappingService identityMappingService,
            IContentLoader contentLoader,
            IAprimoAssetPersistantService aprimoAssetPersistantService,
            IAprimoAssetService aprimoAssetService,
            IContentCacheRemover contentCacheRemover,
            AprimoContentModelStore aprimoContentModelStore)
        {
            this.identityMappingService = identityMappingService;
            this.contentLoader = contentLoader;
            this.aprimoAssetPersistantService = aprimoAssetPersistantService;
            this.aprimoAssetService = aprimoAssetService;
            this.contentCacheRemover = contentCacheRemover;
            this.aprimoContentModelStore = aprimoContentModelStore;
        }

        [HttpGet]
        public ActionResult Get(string id, string renditionId, string publicUrl, string title, string ownerContentItemId)
        {
            Uri externalIdentifier = MappedIdentity.ConstructExternalIdentifier(AprimoConstants.ProviderKey, $"{id}/{renditionId}");
            MappedIdentity mappedIdentity = this.identityMappingService.Get(externalIdentifier, true);

            //Guid assetId = Guid.Empty;
            //Guid assetRenditionId = Guid.Empty;

            if (Guid.TryParse(id, out Guid assetId) && Guid.TryParse(renditionId, out Guid assetRenditionId))
            {
                // checking that we actually have guids.
                var persistantAsset = this.aprimoAssetPersistantService.GetAsset(mappedIdentity.ContentLink);

                if (persistantAsset == null)
                {
                    persistantAsset = new AprimoPersistantAsset(mappedIdentity.ContentLink, assetId, renditionId)
                    {
                        CDNUrl = publicUrl,
                        Title = title
                    };

                    var thumbnail = UriUtil.AddQueryString(publicUrl, "width", "180");
                    thumbnail = UriUtil.AddQueryString(thumbnail, "height", "100");
                    persistantAsset.ThumbnailUrl = thumbnail;

                    var fileAsset = this.aprimoAssetService.GetAsset(id);
                    if (fileAsset != null)
                    {
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
                    }

                    this.aprimoAssetPersistantService.Save(persistantAsset);
                }
            }
            // Put in Task since we don't need this to be returned.
            Task.Run(() =>
            {
                try
                {
                    if (ContentReference.TryParse(ownerContentItemId, out ContentReference ownerContentLink))
                    {
                        this.aprimoAssetService.CreatePermalinkForAprimo(id, mappedIdentity.ContentLink, ownerContentLink.ToReferenceWithoutVersion());
                    }
                    else
                    {
                        this.aprimoAssetService.CreatePermalinkForAprimo(id, mappedIdentity.ContentLink, ContentReference.EmptyReference);
                    }
                }
                catch (Exception)
                {
                }
            });
            var assetData = this.contentLoader.Get<AprimoAssetData>(mappedIdentity.ContentLink);
            var assetContentModel = this.aprimoContentModelStore.Create(assetData);

            return Rest(assetContentModel);
        }
    }
}