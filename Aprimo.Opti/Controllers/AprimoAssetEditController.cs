using Aprimo.Opti.Models;
using Aprimo.Opti.Resolvers;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Web;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Aprimo.Opti.Controllers
{
    [TemplateDescriptor(
        Inherited = true,
        AvailableWithoutTag = false,
        Tags = new[] { RenderingTags.Edit, RenderingTags.Preview },
        TemplateTypeCategory = TemplateTypeCategories.MvcController)]
    public class AprimoAssetEditController : PartialContentController<AprimoAssetData>
    {
        private readonly UrlResolver urlResolver;

        private const string PermalinkTemplate = "https://{0}.dam.aprimo.com/dam/contentitems/{1}";

        private readonly IAprimoFieldPropertyResolver aprimoFieldPropertyResolver;

        public AprimoAssetEditController(UrlResolver urlResolver, IAprimoFieldPropertyResolver aprimoFieldPropertyResolver)
        {
            this.urlResolver = urlResolver;
            this.aprimoFieldPropertyResolver = aprimoFieldPropertyResolver;
        }

        public override ActionResult Index(AprimoAssetData currentContent)
        {
            string aprimoUrl = string.Format(PermalinkTemplate, AprimoAPISettings.AprimoTenantId, currentContent.AssetId);
            StringBuilder sb = new StringBuilder("<link rel=\"stylesheet\" href=\"https://cdn.jsdelivr.net/npm/bootstrap@5.1.0/dist/css/bootstrap.min.css\" integrity =\"sha384-KyZXEAg3QhqLMpG8r+8fhAXLRk2vvoC2f3B09zVXn8CA5QIVfZOJ3BCsw2P0p/We\" crossorigin=\"anonymous\">");
            sb.Append("<div class=\"container py-5\">");
            sb.Append("<div class=\"row\">");
            sb.Append("<div class=\"col-md-8\">");

            if (currentContent is IAprimoImage aprimoImageFile)
            {
                TagBuilder builder = new TagBuilder("img");
                builder.MergeAttribute("class", "img-fluid");
                builder.MergeAttribute("src", aprimoImageFile.PreviewUrl);
                sb.Append(builder.ToString(TagRenderMode.SelfClosing));
            }

            sb.Append("</div>");
            sb.Append("<div class=\"col-md-4\">");

            TagBuilder dl = new TagBuilder("dl");
            dl.MergeAttribute("class", "row");
            dl.InnerHtml += DescriptionList("Aprimo Id", currentContent.AssetId).ToString();

            var aprimoLinkBuilder = new TagBuilder("a");
            aprimoLinkBuilder.SetInnerText("View in Aprimo");
            aprimoLinkBuilder.MergeAttribute("href", aprimoUrl);
            aprimoLinkBuilder.MergeAttribute("target", "_blank");
            dl.InnerHtml += DescriptionListWithHtml("Asset Url", aprimoLinkBuilder.ToString()).ToString();

            // Fields
            var fieldProperties = this.aprimoFieldPropertyResolver.Resolve(typeof(AprimoAssetData));
            if (fieldProperties.Any())
            {
                foreach (var fieldProperty in fieldProperties)
                {
                    if (currentContent[fieldProperty.Name] != null)
                    {
                        dl.InnerHtml += DescriptionList(fieldProperty.Name, currentContent.GetPropertyValue(fieldProperty.Name)).ToString();
                    }
                }
            }

            sb.Append(dl.ToString());
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");
            return base.Content(sb.ToString());
        }

        private StringBuilder DescriptionList(string title, string description)
        {
            var descriptionBuilder = new StringBuilder();

            var dtBuilder = new TagBuilder("dt");
            dtBuilder.MergeAttribute("class", "col-sm-3");
            dtBuilder.SetInnerText($"{title}:");
            descriptionBuilder.AppendLine(dtBuilder.ToString());

            var ddBuilder = new TagBuilder("dd");
            ddBuilder.MergeAttribute("class", "col-sm-9");
            ddBuilder.SetInnerText(description);
            descriptionBuilder.AppendLine(ddBuilder.ToString());
            return descriptionBuilder;
        }

        private StringBuilder DescriptionListWithHtml(string title, string description)
        {
            var descriptionBuilder = new StringBuilder();

            var dtBuilder = new TagBuilder("dt");
            dtBuilder.MergeAttribute("class", "col-sm-3");
            dtBuilder.SetInnerText($"{title}:");
            descriptionBuilder.AppendLine(dtBuilder.ToString());

            var ddBuilder = new TagBuilder("dd");
            ddBuilder.MergeAttribute("class", "col-sm-9");
            ddBuilder.InnerHtml += description;
            descriptionBuilder.AppendLine(ddBuilder.ToString());
            return descriptionBuilder;
        }

        private string GenerateUrl(string url, int width, int height)
        {
            var thumbUrl = UriUtil.AddQueryString(url, "width", width.ToString());
            thumbUrl = UriUtil.AddQueryString(thumbUrl, "height", height.ToString());
            return thumbUrl;
        }
    }
}