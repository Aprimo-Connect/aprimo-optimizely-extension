using Newtonsoft.Json;

namespace Aprimo.Opti.Models.RestImpl
{
    public class AssetItemLinks
    {
        [JsonProperty("fields")]
        public AssetAncestors Fields { get; set; }

        [JsonProperty("files")]
        public AssetAncestors Files { get; set; }

        [JsonProperty("masterfile")]
        public AssetAncestors Masterfile { get; set; }

        [JsonProperty("masterfilelatestversion")]
        public AssetAncestors Masterfilelatestversion { get; set; }

        [JsonProperty("classifications")]
        public AssetAncestors Classifications { get; set; }

        [JsonProperty("parent")]
        public AssetAncestors Parent { get; set; }

        [JsonProperty("ancestors")]
        public AssetAncestors Ancestors { get; set; }

        [JsonProperty("children")]
        public AssetAncestors Children { get; set; }

        [JsonProperty("permissions")]
        public AssetAncestors Permissions { get; set; }

        [JsonProperty("modifiedby")]
        public AssetAncestors Modifiedby { get; set; }

        [JsonProperty("createdby")]
        public AssetAncestors Createdby { get; set; }
    }
}