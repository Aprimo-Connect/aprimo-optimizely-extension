using Newtonsoft.Json;
using System;

namespace Aprimo.Opti.Models.RestImpl
{
    public partial class Preview
    {
        [JsonProperty("_links")]
        public AssetItemLinks Links { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("extension")]
        public string Extension { get; set; }

        [JsonProperty("uri")]
        public Uri Uri { get; set; }

        [JsonProperty("actualPath")]
        public object ActualPath { get; set; }
    }
}