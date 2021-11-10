using Aprimo.Opti.Attributes;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace Aprimo.Opti.Models
{
    [ContentType(DisplayName = "Aprimo Generic File", GUID = "59e186f1-a5eb-4615-a7dd-29d1c9b435fb", Description = "")]
    [AprimoAssetDescriptor(ExtensionString = "*")]
    public class AprimoGenericFile : AprimoAssetData
    {
    }
}