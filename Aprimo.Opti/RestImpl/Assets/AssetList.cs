using Newtonsoft.Json;

namespace Aprimo.Opti.Models.RestImpl
{
    public class AssetList : AprimoModelListBase<Asset>
    {
        [JsonProperty("_links")]
        public AssetItemLinks Links { get; set; }
    }
}