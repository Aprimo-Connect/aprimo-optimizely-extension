using Aprimo.Opti.Models;
using EPiServer.Shell;

namespace Aprimo.Opti.Descriptors
{
    [UIDescriptorRegistration]
    public class AprimoAssetUIDescriptor : UIDescriptor<AprimoAssetData>, IEditorDropBehavior
    {
        public EditorDropBehavior EditorDropBehaviour { get; set; }

        public AprimoAssetUIDescriptor()
            : base("epi-iconObjectContainerFetchContent")
        {
            EditorDropBehaviour = EditorDropBehavior.CreateLink;
            DefaultView = CmsViewNames.PreviewView;
        }
    }
}