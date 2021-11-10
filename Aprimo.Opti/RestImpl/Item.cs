using Newtonsoft.Json;
using System;

namespace Aprimo.Opti.Models.RestImpl
{
    public partial class Item
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("isManual")]
        public bool IsManual { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("fileSize")]
        public long FileSize { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("extension")]
        public string Extension { get; set; }

        [JsonProperty("tag")]
        public object Tag { get; set; }

        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        [JsonProperty("actualPath")]
        public object ActualPath { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty("purposes")]
        public string[] Purposes { get; set; }
    }
}