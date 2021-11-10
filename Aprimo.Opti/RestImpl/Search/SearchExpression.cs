using Newtonsoft.Json;

namespace Aprimo.Opti.Models.RestImpl.Search
{
    public partial class SearchRequest
    {
        public SearchRequest()
        {
            this.PageIndex = 1;
            this.PageSize = 50;
        }

        [JsonProperty("searchExpression")]
        public SearchExpression SearchExpression { get; set; }

        [JsonProperty("logRequest")]
        public bool LogRequest { get; set; }

        [JsonIgnore]
        public int PageIndex { get; set; }

        [JsonIgnore]
        public int PageSize { get; set; }
    }

    public partial class SearchExpression
    {
        [JsonProperty("expression")]
        public string Expression { get; set; }
    }
}