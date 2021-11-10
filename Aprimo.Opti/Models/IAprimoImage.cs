namespace Aprimo.Opti.Models
{
    public interface IAprimoImage
    {
        /// <summary>
        /// The images rendition id from cdn.
        /// </summary>
        string RenditionId { get; set; }

        /// <summary>
        /// The actual asset url
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// Used internally for thumbnails in asset pane.
        /// </summary>
        string ThumbnailUrl { get; set; }

        /// <summary>
        /// Used internally for thumbnails in properties.
        /// </summary>
        string ThumbnailPreviewUrl { get; set; }

        /// <summary>
        /// Used internally for images display in edit mode.
        /// </summary>
        string PreviewUrl { get; set; }
    }
}