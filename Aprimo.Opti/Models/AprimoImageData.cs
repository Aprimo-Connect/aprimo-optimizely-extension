using Aprimo.Opti.Attributes;

namespace Aprimo.Opti.Models
{
    public abstract class AprimoImageData : AprimoAssetData, IAprimoImage
    {
        public virtual string RenditionId { get; set; }

        /// <summary>
        /// Used for thumbnail in component
        /// </summary>
        [AprimoTransform(Height = "100", Width = "100")]
        public virtual string ThumbnailUrl { get; set; }

        /// <summary>
        /// Used for Preview in Url and ContentReference properties
        /// </summary>
        [AprimoTransform(Width = "180", Crop = "16:9")]
        public virtual string ThumbnailPreviewUrl { get; set; }

        /// <summary>
        /// Used for Preview in Preview Mode
        /// </summary>
        [AprimoTransform(Width = "1024", Auto = "webp")]
        public virtual string PreviewUrl { get; set; }
    }
}