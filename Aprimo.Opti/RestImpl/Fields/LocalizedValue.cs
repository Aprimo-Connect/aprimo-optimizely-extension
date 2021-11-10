using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aprimo.Opti.Models.RestImpl.Fields
{
    public partial class LocalizedValue
    {
        public LocalizedValue()
        {
            this.LanguageId = "00000000000000000000000000000000";
            this.Values = new List<string>();
        }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("languageId")]
        public string LanguageId { get; set; }

        [JsonProperty("values")]
        public List<string> Values { get; set; }
    }
}