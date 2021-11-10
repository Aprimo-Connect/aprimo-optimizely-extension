using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Aprimo.Opti.Models.RestImpl
{
    public class Asset : AprimoModelBase
    {
        [JsonProperty("_links")]
        public AssetItemLinks Links { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("_embedded")]
        public Embedded Embedded { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JToken> PropertyOptionsDictionary { get; set; }
    }
}