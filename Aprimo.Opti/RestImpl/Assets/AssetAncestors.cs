using Newtonsoft.Json;
using System;

namespace Aprimo.Opti.Models.RestImpl
{
    public partial class AssetAncestors
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }

        [JsonProperty("select-key")]
        public AssetSelectKey SelectKey { get; set; }
    }
}