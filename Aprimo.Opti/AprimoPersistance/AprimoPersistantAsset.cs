using EPiServer.Core;
using System;

namespace Aprimo.Opti.AprimoPersistance
{
    public class AprimoPersistantAsset
    {
        public AprimoPersistantAsset()
        {
        }

        public AprimoPersistantAsset(ContentReference contentReference)
        {
            this.Id = contentReference.ToReferenceWithoutVersion().ID;
        }

        public AprimoPersistantAsset(ContentReference id, Guid assetId, string renditionId)
            : this(id)
        {
            this.AssetId = assetId;
            this.RenditionId = renditionId;
        }

        public int Id { get; set; }

        public Guid AssetId { get; set; }

        public string RenditionId { get; set; }

        public string Title { get; set; }

        public string CDNUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public string MetaInformation { get; set; }

        public string Extension { get; set; }

        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}