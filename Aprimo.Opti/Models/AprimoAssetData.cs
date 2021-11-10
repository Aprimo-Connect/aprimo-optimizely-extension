using Aprimo.Opti.Attributes;
using EPiServer.Core;
using System.Globalization;

namespace Aprimo.Opti.Models
{
    public abstract class AprimoAssetData : ContentBase, IContent, IContentData, ILocale, IAprimoAsset
    {
        public virtual string AssetId { get; set; }

        public virtual string Url { get; set; }

        [AprimoFieldName("Title")]
        public virtual string Title { get; set; }

        [AprimoFieldName("Description")]
        public virtual string Description { get; set; }

        [AprimoFieldName(AprimoConstants.AprimoEPiPermalinkFieldName)]
        public virtual string EPiPermalink { get; set; }

        public virtual string AssetType { get; set; }

        public virtual string FileExtension { get; set; }

        public CultureInfo Language { get; set; }
    }
}