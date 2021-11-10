using Aprimo.Opti.AprimoPersistance;
using Aprimo.Opti.Models;
using Aprimo.Opti.Services;
using EPiServer;
using EPiServer.Cms.Shell.Search;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Localization;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.Search;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;

namespace Aprimo.Opti
{
    [SearchProvider]
    public class AprimoSearchProvider : ContentSearchProviderBase<AprimoAssetData, ContentType>
    {
        private readonly IAprimoAssetPersistantService aprimoAssetPersistantService;

        private readonly IContentLoader contentLoader;

        private readonly IdentityMappingService identityMappingService;

        public AprimoSearchProvider(LocalizationService localizationService,
            ISiteDefinitionResolver siteDefinitionResolver,
            IContentTypeRepository contentTypeRepository,
            EditUrlResolver editUrlResolver,
            ServiceAccessor<SiteDefinition> serviceAccessor,
            LanguageResolver languageResolver,
            UrlResolver urlResolver,
            TemplateResolver templateResolver,
            UIDescriptorRegistry uiDescriptorRegistry,
            IAprimoAssetPersistantService aprimoAssetPersistantService,
            IContentLoader contentLoader,
            IdentityMappingService identityMappingService)
           : base(localizationService, siteDefinitionResolver, contentTypeRepository, editUrlResolver, serviceAccessor, languageResolver, urlResolver, templateResolver, uiDescriptorRegistry)
        {
            this.aprimoAssetPersistantService = aprimoAssetPersistantService;
            this.contentLoader = contentLoader;
            this.identityMappingService = identityMappingService;
        }

        public override IEnumerable<SearchResult> Search(Query query)
        {
            var assets = this.aprimoAssetPersistantService.Search(query.SearchQuery);

            foreach (var asset in assets)
            {
                yield return this.CreateSearchResult(GetAsset(asset));
            }
        }

        private AprimoAssetData GetAsset(AprimoPersistantAsset asset)
        {
            Uri externalIdentifier = MappedIdentity.ConstructExternalIdentifier(AprimoConstants.ProviderKey, $"{asset.AssetId}/{asset.RenditionId}");
            MappedIdentity mappedIdentity = this.identityMappingService.Get(externalIdentifier, true);
            return this.contentLoader.Get<AprimoAssetData>(mappedIdentity.ContentLink);
        }

        public override string Area => AprimoConstants.ProviderKey;

        public override string Category => AprimoConstants.ProviderName;

        protected override string IconCssClass => "epi-iconObjectImage";
    }
}