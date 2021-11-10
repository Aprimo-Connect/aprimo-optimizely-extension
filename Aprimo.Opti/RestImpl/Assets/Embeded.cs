using Aprimo.Opti.Models.RestImpl.Fields;
using Newtonsoft.Json;

namespace Aprimo.Opti.Models.RestImpl
{
    public partial class Embedded
    {
        public Embedded()
        {
            this.MasterFileVersion = new MasterFileversionList();
        }

        [JsonProperty("preview")]
        public Preview Preview { get; set; }

        [JsonProperty("thumbnail")]
        public Preview Thumbnail { get; set; }

        [JsonProperty("masterfilelatestversion")]
        public MasterFileversionList MasterFileVersion { get; set; }

        [JsonProperty("fields")]
        public Field Fields { get; set; }
    }
}