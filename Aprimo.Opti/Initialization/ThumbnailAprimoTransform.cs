using Aprimo.Opti.Models;
using EPiServer.Cms.Shell.UI.Rest.Models;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms.Internal;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Globalization;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System.Collections.Generic;

namespace Aprimo.Opti.Initialization
{
    public class ThumbnailAprimoTransform : StructureStoreModelTransform
    {
        public ThumbnailAprimoTransform(IContentProviderManager contentProviderManager,
            ILanguageBranchRepository languageBranchRepository,
            IContentLanguageSettingsHandler contentLanguageSettingsHandler,
            ISiteDefinitionRepository siteDefinitionRepository,
            LanguageResolver languageResolver,
            IEnumerable<IHasChildrenEvaluator> hasChildrenEvaluator,
            TemplateResolver templateResolver,
            UrlResolver urlResolver) :
            base(contentProviderManager, languageBranchRepository, contentLanguageSettingsHandler, siteDefinitionRepository, hasChildrenEvaluator, languageResolver, urlResolver, templateResolver)
        {
        }

        public override void TransformInstance(IContent source, StructureStoreContentDataModel target, IModelTransformContext context)
        {
            base.TransformInstance(source, target, context);
            if (source is IAprimoImage aprimoThumbnailItem)
            {
                target.ThumbnailUrl = aprimoThumbnailItem.ThumbnailUrl;
            }
        }
    }
}