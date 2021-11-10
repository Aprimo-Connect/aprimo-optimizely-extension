using Newtonsoft.Json;
using System;

namespace Aprimo.Opti.Models.RestImpl
{
    public class MasterFileversionList : AprimoModelBase
    {
        public MasterFileversionList()
        {
        }

        [JsonProperty("_links")]
        public AssetItemLinks Links { get; set; }

        [JsonProperty("versionLabel")]
        public string VersionLabel { get; set; }

        [JsonProperty("versionNumber")]
        public long VersionNumber { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("fileCreatedOn")]
        public DateTimeOffset FileCreatedOn { get; set; }

        [JsonProperty("fileModifiedOn")]
        public DateTimeOffset FileModifiedOn { get; set; }

        [JsonProperty("metadata")]
        public object Metadata { get; set; }

        [JsonProperty("fileSize")]
        public long FileSize { get; set; }

        [JsonProperty("fileExtension")]
        public string FileExtension { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("crc32")]
        public long Crc32 { get; set; }

        [JsonProperty("content")]
        public object Content { get; set; }

        [JsonProperty("preventDownload")]
        public bool PreventDownload { get; set; }

        [JsonProperty("fileState")]
        public AssetFileState FileState { get; set; }

        [JsonProperty("actualPath")]
        public object ActualPath { get; set; }

        [JsonProperty("_embedded")]
        public Embedded Embedded { get; set; }
    }
}