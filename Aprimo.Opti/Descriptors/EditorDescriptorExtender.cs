using EPiServer.Cms.Shell.UI.ObjectEditing.Internal;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.ObjectEditing;
using System;
using System.Collections.Generic;

namespace Aprimo.Opti.Descriptors
{
    public static class EditorDescriptorExtender
    {
        private static readonly Injected<FileExtensionsResolver> fileExtensionsResolver;

        public static void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            metadata.EditorConfiguration["aprimoSelectContentUrl"] = $"https://{AprimoAPISettings.AprimoTenantId}.dam.aprimo.com";
            metadata.EditorConfiguration["aprimoDialogMode"] = AprimoAPISettings.DialogMode;
            metadata.EditorConfiguration["aprimoLabelButton"] = AprimoAPISettings.LabelButton;
            metadata.EditorConfiguration["aprimoTitle"] = AprimoAPISettings.Title;
            metadata.EditorConfiguration["aprimoDescription"] = AprimoAPISettings.Description;
            metadata.EditorConfiguration["aprimoiconpath"] = "/episerver/aprimo.opti/1.0.0/clientresources/scripts/editors/images/aprimo_opt_cropped.png";
            metadata.EditorConfiguration["allowedExtensions"] = fileExtensionsResolver.Service.GetAllowedExtensions(typeof(IContentImage), metadata.Attributes);
            metadata.EditorConfiguration["aprimomodulepath"] = Paths.ToResource("Aprimo.Opti", string.Empty);
        }
    }
}