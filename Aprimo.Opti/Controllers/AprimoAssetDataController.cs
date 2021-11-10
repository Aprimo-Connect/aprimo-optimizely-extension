using Aprimo.Opti.Models;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using System.Web.Mvc;

namespace Aprimo.Opti.Controllers
{
    [TemplateDescriptor(AvailableWithoutTag = true, Inherited = true)]
    public class AprimoAssetDataController : PartialContentController<AprimoAssetData>
    {
        public override ActionResult Index(AprimoAssetData currentContent) =>
            PartialView(currentContent);
    }
}