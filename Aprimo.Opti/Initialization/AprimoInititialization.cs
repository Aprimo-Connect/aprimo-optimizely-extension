using Aprimo.Opti.Services;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System.Collections.Specialized;
using System.Web;
using System.Web.Routing;

namespace Aprimo.Opti.Initialization
{
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class AprimoInititialization : IInitializableModule, IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.ConfigurationComplete += (o, e) =>
            {
                context.Services
                .AddSingleton<IModelTransform, ThumbnailAprimoTransform>();
            };
        }

        public void Initialize(InitializationEngine context)
        {
            context.Locate.Advanced.GetInstance<AprimoContentAssetResolver>()
                .Initialize(context.Locate.Advanced.GetInstance<ContentTypeModelRepository>());

            // Make sure database is up to date.
            var aprimoAssetPersistantService = context.Locate.Advanced.GetInstance<IAprimoAssetPersistantService>();
            aprimoAssetPersistantService.CheckDBConsistancy(AprimoConstants.DBVersion);

            // Create provider root if not exists
            var aprimoRoot = AprimoContentProvider.GetRoot();

            // Load provider
            var contentProviderManager = context.Locate.Advanced.GetInstance<IContentProviderManager>();
            var configValues = new NameValueCollection { { ContentProviderElement.EntryPointString, aprimoRoot.ToString() } };
            var provider = context.Locate.Advanced.GetInstance<AprimoContentProvider>();
            provider.Initialize(AprimoConstants.ProviderKey, configValues);
            contentProviderManager.ProviderMap.AddProvider(provider);

            // Add Routes
            RouteTable.Routes.MapContentRoute(
                name: "AprimoMedia",
                url: "{language}/" + AprimoConstants.ProviderKey + "/{node}/{partial}/{action}",
                defaults: new { action = "index" },
                contentRootResolver: (x) => aprimoRoot);

            // Add Edit Mode Url Route
            RouteTable.Routes.MapContentRoute(
                name: "AprimoMediaEdit",
                url: CmsHomePath + AprimoConstants.ProviderKey + "/{language}/{medianodeedit}/{partial}/{action}",
                defaults: new { action = "index" },
                contentRootResolver: (x) => aprimoRoot);
        }

        private static string CmsHomePath
        {
            get
            {
                var vpue = ServiceLocator.Current.GetInstance<IVirtualPathResolver>();
                var cmsContentPath = VirtualPathUtility.AppendTrailingSlash(EPiServer.Shell.Paths.ToResource("CMS", "Content"));
                return vpue.ToAppRelative(cmsContentPath).Substring(1);
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}