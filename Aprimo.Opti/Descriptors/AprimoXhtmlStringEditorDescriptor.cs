using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using System;
using System.Collections.Generic;

namespace Aprimo.Opti.Descriptors
{
    [EditorDescriptorRegistration(TargetType = typeof(XhtmlString), EditorDescriptorBehavior = EditorDescriptorBehavior.PlaceLast)]
    public class AprimoXhtmlStringEditorDescriptor : EditorDescriptor
    {
        public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
        {
            base.ModifyMetadata(metadata, attributes);

            //FrontifyProjectAttribute frontifyProjectAttribute = attributes.FirstOrDefault((Attribute x) => x is FrontifyProjectAttribute) as FrontifyProjectAttribute;
            //if (frontifyProjectAttribute == null)
            //{
            //	return;
            //}
            //Dictionary<string, object> dictionary = metadata.EditorConfiguration["settings"] as Dictionary<string, object>;
            //if (dictionary != null)
            //{
            //	dictionary["choosermode"] = frontifyProjectAttribute.UiMode;
            //	if (!string.IsNullOrEmpty(frontifyProjectAttribute.ProjectGuid))
            //	{
            //		dictionary["apitoken"] = frontifyProjectAttribute.ProjectGuid;
            //	}
            //	if (!string.IsNullOrWhiteSpace(frontifyProjectAttribute.AllowedFileTypes))
            //	{
            //		dictionary["frontifyallowedfiletypes"] = frontifyProjectAttribute.AllowedFileTypes;
            //	}
            //}
        }
    }
}