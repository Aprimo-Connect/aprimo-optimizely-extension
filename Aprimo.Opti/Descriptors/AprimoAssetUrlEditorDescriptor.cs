using EPiServer;
using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;
using EPiServer.Cms.Shell.UI.ObjectEditing.Internal;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using System;
using System.Collections.Generic;

namespace Aprimo.Opti.Descriptors
{
    //[EditorDescriptorRegistration(TargetType = typeof(Url), UIHint = UIHint.Image)]
    //[EditorDescriptorRegistration(TargetType = typeof(Url), UIHint = AprimoUIHints.AprimoImage)]
    [EditorDescriptorRegistration(TargetType = typeof(Url), UIHint = AprimoUIHints.AprimoImageUrl)]
    public class AprimoAssetUrlEditorDescriptor : ImageUrlEditorDescriptor
    {
        //private readonly Injected<AprimoConnectorSettings> AprimoConnectorSettings;

        public AprimoAssetUrlEditorDescriptor(FileExtensionsResolver fileExtensionsResolver)
            : base(fileExtensionsResolver)
        {
        }

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            base.ClientEditingClass = "aprimointegration/editors/aprimourlthumbnailselector";
            EditorDescriptorExtender.ModifyMetadata(metadata, attributes);
            base.ModifyMetadata(metadata, attributes);
        }
    }
}