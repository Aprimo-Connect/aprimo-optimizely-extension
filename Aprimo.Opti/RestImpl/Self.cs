using Newtonsoft.Json;
using System;

namespace Aprimo.Opti.Models.RestImpl
{
    public class Self
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }
    }
}