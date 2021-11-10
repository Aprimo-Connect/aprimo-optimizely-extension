using Aprimo.Opti.Models;
using EPiServer.Shell;

namespace Aprimo.Opti.Descriptors
{
    [UIDescriptorRegistration]
    public class AprimoAssetImageUIDescriptor : UIDescriptor<IAprimoImage>, IEditorDropBehavior
    {
        public EditorDropBehavior EditorDropBehaviour { get; set; }

        public AprimoAssetImageUIDescriptor()
            : base("epi-iconObjectImage")
        {
            EditorDropBehaviour = EditorDropBehavior.CreateImage;
            DefaultView = CmsViewNames.PreviewView;
        }
    }
}