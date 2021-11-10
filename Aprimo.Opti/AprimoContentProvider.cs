using Aprimo.Opti.Attributes;
using Aprimo.Opti.Models;
using Aprimo.Opti.Resolvers;
using Aprimo.Opti.Services;
using EPiServer;
using EPiServer.Construction;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Logging.Compatibility;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Aprimo.Opti
{
    public class AprimoContentProvider : ContentProvider
    {
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IdentityMappingService identityMappingService;

        private readonly IAprimoAssetService aprimoAssetService;

        private readonly IContentTypeRepository contentTypeRepository;

        private readonly IContentLoader contentLoader;

        private readonly IContentFactory contentFactory;

        private readonly IAprimoAssetPersistantService aprimoAssetPersistantService;

        private readonly IAprimoTransformDescriptorPropertyResolver aprimoTransformDescriptorPropertyResolver;

        private readonly IAprimoFieldPropertyResolver aprimoFieldPropertyResolver;

        private readonly AprimoContentAssetResolver aprimoContentAssetResolver;

        public override string ProviderKey => AprimoConstants.ProviderKey;

        public AprimoContentProvider(
            IAprimoAssetService aprimoAssetService,
            IdentityMappingService identityMappingService,
            IContentTypeRepository contentTypeRepository,
            IContentFactory contentFactory,
            IContentLoader contentLoader,
            IAprimoAssetPersistantService aprimoAssetPersistantService,
            IAprimoTransformDescriptorPropertyResolver aprimoTransformDescriptorPropertyResolver,
            AprimoFieldPropertyResolver aprimoFieldPropertyResolver,
            AprimoContentAssetResolver aprimoContentAssetResolver)
        {
            this.identityMappingService = identityMappingService;
            this.contentTypeRepository = contentTypeRepository;
            this.aprimoAssetService = aprimoAssetService;
            this.contentFactory = contentFactory;
            this.contentLoader = contentLoader;
            this.aprimoAssetPersistantService = aprimoAssetPersistantService;
            this.aprimoTransformDescriptorPropertyResolver = aprimoTransformDescriptorPropertyResolver;
            this.aprimoFieldPropertyResolver = aprimoFieldPropertyResolver;
            this.aprimoContentAssetResolver = aprimoContentAssetResolver;
        }

        protected override IContent LoadContent(ContentReference contentLink, ILanguageSelector languageSelector)
        {
            MappedIdentity mappedIdentity = identityMappingService.Get(contentLink);
            IContent currentContent = null;

            if (mappedIdentity == null)
            {
                return null;
            }

            try
            {
                var renditionId = mappedIdentity.ExternalIdentifier.Segments.Last();
                var masterId = mappedIdentity.ExternalIdentifier.Segments.ElementAt(1).Trim(new char[] { '/' });
                var file = this.aprimoAssetService.GetAsset(masterId);
                string extension = string.Empty;
                if (file != null)
                {
                    ContentType contentType = this.contentTypeRepository.Load(typeof(AprimoGenericFile));
                    if (file?.Embedded?.MasterFileVersion?.FileExtension != null)
                    {
                        extension = file?.Embedded?.MasterFileVersion?.FileExtension;
                        var modelType = this.aprimoContentAssetResolver.GetFirstMatching(extension);
                        contentType = contentTypeRepository.Load(modelType);
                    }

                    AprimoAssetData assetData = contentFactory.CreateContent(contentType, new BuildingContext(contentType)
                    {
                        Parent = contentLoader.Get<ContentFolder>(this.EntryPoint)
                    }) as AprimoAssetData;

                    assetData.AssetId = masterId;
                    assetData.ContentTypeID = contentType.ID;
                    assetData.Name = file?.Embedded?.MasterFileVersion.FileName;
                    assetData.FileExtension = extension;
                    assetData.Created = file.CreatedOn.Date;
                    assetData.ContentGuid = mappedIdentity.ContentGuid;
                    assetData.ContentLink = mappedIdentity.ContentLink;
                    assetData.AssetType = file.ContentType;

                    if (assetData is IVersionable versionable)
                    {
                        versionable.IsPendingPublish = false;
                        versionable.Status = VersionStatus.Published;
                        versionable.StartPublish = file.CreatedOn.Date;
                    }

                    if (assetData is IContentSecurable securable)
                    {
                        securable.GetContentSecurityDescriptor()
                            .AddEntry(new AccessControlEntry(EveryoneRole.RoleName, AccessLevel.Read));
                    }

                    if (assetData is IChangeTrackable changeTrackable)
                    {
                        changeTrackable.Changed = DateTime.Now;
                    }

                    #region Field Population

                    // populate Fields for all assets
                    if (file.Embedded.Fields.Items.Any())
                    {
                        var fieldProperties = this.aprimoFieldPropertyResolver.Resolve(typeof(AprimoAssetData));
                        foreach (var fieldPropertyInfo in fieldProperties)
                        {
                            var attrs = fieldPropertyInfo.GetCustomAttributes(true).OfType<AprimoFieldNameAttribute>();
                            foreach (AprimoFieldNameAttribute fieldAttr in attrs)
                            {
                                var value = file.GetFieldValue(fieldAttr.FieldName);
                                if (!string.IsNullOrWhiteSpace(value))
                                {
                                    assetData.SetValue(fieldPropertyInfo.Name, value);
                                }
                            }
                        }
                    }

                    #endregion Field Population

                    #region Populate based on Image

                    if (assetData is IAprimoImage aprimoImageAsset)
                    {
                        aprimoImageAsset.RenditionId = renditionId;
                        var asset = this.aprimoAssetPersistantService.GetAsset(Guid.Parse(masterId), renditionId);
                        if (asset != null)
                        {
                            aprimoImageAsset.Url = asset.CDNUrl;

                            if (!string.IsNullOrWhiteSpace(asset.Title))
                            {
                                assetData.Name = asset.Title.Trim();
                            }
                            // populate thumbnails
                            var properties = this.aprimoTransformDescriptorPropertyResolver.Resolve(contentType.ModelType);
                            foreach (var props in properties)
                            {
                                var attrs = props.GetCustomAttributes(true).OfType<AprimoTransformAttribute>();
                                foreach (AprimoTransformAttribute imageSizeAttr in attrs)
                                {
                                    var thumbUrl = asset.CDNUrl;
                                    var attributeProperties = imageSizeAttr.GetType().GetProperties()
                                        .Where(x => !x.Name.Equals("scaffold", StringComparison.OrdinalIgnoreCase) && !x.Name.Equals("typeid", StringComparison.OrdinalIgnoreCase));

                                    var transformValues = new NameValueCollection();
                                    foreach (var prop in attributeProperties)
                                    {
                                        var propVal = prop.GetValue(imageSizeAttr, null);
                                        if (propVal != null)
                                        {
                                            transformValues.Add(prop.Name.ToLower(), propVal.ToString());
                                        }
                                    }
                                    if (transformValues.HasKeys())
                                    {
                                        var query = UriUtil.BuildQueryString(transformValues);
                                        thumbUrl = string.Concat(thumbUrl, "?", query);
                                    }

                                    props.SetValue(assetData, thumbUrl, null);
                                }
                            }
                        }
                    }

                    #endregion Populate based on Image

                    this.AddContentToCache(assetData);

                    return assetData;
                }
            }
            catch (Exception ex)
            {
                this.log.Error("Error locating Aprimo asset", ex);
            }
            return currentContent;
        }

        protected override IList<GetChildrenReferenceResult> LoadChildrenReferencesAndTypes(ContentReference contentLink, string languageID, out bool languageSpecific)
        {
            languageSpecific = false;
            var childrenList = new List<GetChildrenReferenceResult>();

            if (EntryPoint.CompareToIgnoreWorkID(contentLink))
            {
                var allAssets = this.identityMappingService.List(AprimoConstants.ProviderKey);
                foreach (var asset in allAssets)
                {
                    var childAsset = new GetChildrenReferenceResult()
                    {
                        ContentLink = asset.ContentLink,
                        IsLeafNode = true,
                        ModelType = typeof(AprimoAssetData)
                    };
                    try
                    {
                        var persistantAsset = this.aprimoAssetPersistantService.GetAsset(asset.ContentLink);
                        if (persistantAsset != null)
                        {
                            if (!string.IsNullOrWhiteSpace(persistantAsset.Extension))
                            {
                                childAsset.ModelType = this.aprimoContentAssetResolver.GetFirstMatching(persistantAsset.Extension);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }

                    childrenList.Add(childAsset);
                }
            }
            return childrenList;
        }

        protected override ContentResolveResult ResolveContent(ContentReference contentLink)
        {
            // Check to see fi this is our content
            if (contentLink.ProviderName != this.ProviderKey)
                return null;

            ContentResolveResult contentResolvedResult = new ContentResolveResult
            {
                ContentLink = contentLink
            };
            var content = LoadContent(contentLink, null);
            contentResolvedResult.UniqueID = content.ContentGuid;
            contentResolvedResult.ContentUri = ConstructContentUri(content.ContentTypeID, contentResolvedResult.ContentLink, contentResolvedResult.UniqueID);
            return contentResolvedResult;
        }

        protected override ContentResolveResult ResolveContent(Guid contentGuid)
        {
            var contentItem = this.identityMappingService.Get(contentGuid);
            if (contentItem == null)
                return null;

            ContentResolveResult contentResolvedType = new ContentResolveResult
            {
                ContentLink = contentItem.ContentLink
            };
            var content = LoadContent(contentResolvedType.ContentLink, null);
            contentResolvedType.UniqueID = contentGuid;
            contentResolvedType.ContentUri = ConstructContentUri(content.ContentTypeID, contentResolvedType.ContentLink, contentResolvedType.UniqueID);
            return contentResolvedType;
        }

        protected override IList<MatchingSegmentResult> ListMatchingSegments(ContentReference parentLink, string urlSegment)
        {
            var list = new List<MatchingSegmentResult>();

            bool languageSpecific = false;

            var children = LoadChildrenReferencesAndTypes(parentLink, null, out languageSpecific);

            foreach (var child in children)
            {
                var content = LoadContent(child.ContentLink, null);

                if (content is IRoutable && (content as IRoutable).RouteSegment.Equals(urlSegment, StringComparison.InvariantCultureIgnoreCase))
                {
                    list.Add(new MatchingSegmentResult() { ContentLink = content.ContentLink });
                }
            }

            return list;
        }

        protected override void SetCacheSettings(ContentReference contentReference, IEnumerable<GetChildrenReferenceResult> children, CacheSettings cacheSettings)
        {
            // Set a low cache setting so new items are fetched from data source, but keep the
            // items already fetched for a long time in the cache.
            //cacheSettings.CancelCaching = true;
            cacheSettings.SlidingExpiration = System.Web.Caching.Cache.NoSlidingExpiration;
            cacheSettings.AbsoluteExpiration = DateTime.Now.AddMinutes(5);

            base.SetCacheSettings(contentReference, children, cacheSettings);
        }

        protected override void SetCacheSettings(IContent content, CacheSettings cacheSettings)
        {
            cacheSettings.SlidingExpiration = System.Web.Caching.Cache.NoSlidingExpiration;
            cacheSettings.AbsoluteExpiration = DateTime.Now.AddMinutes(5);
            base.SetCacheSettings(content, cacheSettings);
        }

        public static ContentReference GetRoot()
        {
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var aprimoRoot = contentRepository.GetBySegment(ContentReference.RootPage, AprimoConstants.ProviderKey, LanguageSelector.AutoDetect(true));
            if (aprimoRoot == null)
            {
                aprimoRoot = contentRepository.GetDefault<ContentFolder>(ContentReference.RootPage);
                aprimoRoot.Name = AprimoConstants.ProviderName;
                ((IRoutable)aprimoRoot).RouteSegment = AprimoConstants.ProviderKey;
                return contentRepository.Save(aprimoRoot, SaveAction.Publish, AccessLevel.NoAccess);
            }
            return aprimoRoot.ContentLink;
        }
    }
}