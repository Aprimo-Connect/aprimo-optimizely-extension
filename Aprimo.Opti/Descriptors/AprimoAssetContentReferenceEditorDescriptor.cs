using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;
using EPiServer.Cms.Shell.UI.ObjectEditing.Internal;
using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using EPiServer.Web;
using System;
using System.Collections.Generic;

namespace Aprimo.Opti.Descriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(ContentReference), UIHint = UIHint.Image, EditorDescriptorBehavior = EditorDescriptorBehavior.OverrideDefault)]
    [EditorDescriptorRegistration(TargetType = typeof(ContentReference), UIHint = AprimoUIHints.AprimoImage, EditorDescriptorBehavior = EditorDescriptorBehavior.OverrideDefault)]
    public class AprimoAssetContentReferenceEditorDescriptor : ImageReferenceEditorDescriptor
    {
        public AprimoAssetContentReferenceEditorDescriptor(FileExtensionsResolver fileExtensionsResolver)
            : base(fileExtensionsResolver)
        {
        }

        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            base.ClientEditingClass = "aprimointegration/editors/aprimoassetselector";
            EditorDescriptorExtender.ModifyMetadata(metadata, attributes);
            base.ModifyMetadata(metadata, attributes);
        }
    }
}