using Aprimo.Opti.Models;
using Aprimo.Opti.Services;
using EPiServer;
using EPiServer.Core;
using EPiServer.Shell.Services.Rest;
using System.Web.Mvc;

namespace Aprimo.Opti
{
    [RestStore("aprimoassetstore")]
    public class AprimoAssetStore : RestControllerBase
    {
        private readonly IContentLoader contentLoader;

        private readonly AprimoContentModelStore aprimoContentModelStore;

        public AprimoAssetStore(IContentLoader contentLoader, AprimoContentModelStore aprimoContentModelStore)
        {
            this.contentLoader = contentLoader;
            this.aprimoContentModelStore = aprimoContentModelStore;
        }

        [HttpGet]
        public ActionResult Get(string id)
        {
            var contentLink = new ContentReference(id);
            if (!string.IsNullOrWhiteSpace(contentLink.ProviderName) && contentLink.ProviderName.Equals(AprimoConstants.ProviderKey))
            {
                var content = this.contentLoader.Get<AprimoAssetData>(contentLink);
                var item = this.aprimoContentModelStore.Create(content);
                if (content is AprimoImageData aprimoThumbnail)
                {
                    item.Properties.Add("thumbnailUrl", aprimoThumbnail.ThumbnailUrl);
                }
                return Rest(item);
            }

            return Rest(this.contentLoader.Get<IContent>(contentLink));
        }
    }
}