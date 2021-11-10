using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aprimo.Opti.Models.RestImpl.Fields
{
    public partial class FieldItem
    {
        [JsonProperty("dataType")]
        public string DataType { get; set; }

        [JsonProperty("fieldName")]
        public string FieldName { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("localizedValues")]
        public List<LocalizedValue> LocalizedValues { get; set; }
    }
}