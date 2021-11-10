using Aprimo.Opti.Models.RestImpl.Fields;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aprimo.Opti.RestImpl
{
    public partial class AddUpdateRecordRequest
    {
        public AddUpdateRecordRequest()
        {
            this.Fields = new Fields();
        }

        public AddUpdateRecordRequest(Fields fields)
        {
            this.Fields = fields;
        }

        [JsonProperty("fields")]
        public Fields Fields { get; set; }
    }

    public partial class Fields
    {
        public Fields()
        {
            this.AddOrUpdate = new List<AddOrUpdateField>();
        }

        [JsonProperty("addOrUpdate")]
        public List<AddOrUpdateField> AddOrUpdate { get; set; }

        public void Add(AddOrUpdateField addOrUpdateField)
        {
            this.AddOrUpdate.Add(addOrUpdateField);
        }

        public void Add(string recordId, string value)
        {
            var fieldAddOrUpdate = new AddOrUpdateField(recordId);
            fieldAddOrUpdate.AddValue(value);
            this.Add(fieldAddOrUpdate);
        }

        public void Add(string recordId, List<string> values)
        {
            var fieldAddOrUpdate = new AddOrUpdateField(recordId);
            fieldAddOrUpdate.AddValue(values);
            this.Add(fieldAddOrUpdate);
        }
    }

    public partial class AddOrUpdateField
    {
        private const string DefaultLanguageId = "00000000000000000000000000000000";

        public AddOrUpdateField()
        {
            this.LocalizedValues = new List<LocalizedValue>();
        }

        public AddOrUpdateField(string recordId)
            : this()
        {
            this.Id = recordId;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("localizedValues")]
        public List<LocalizedValue> LocalizedValues { get; set; }

        public void AddValue(string value) =>
            this.AddValue(DefaultLanguageId, value);

        public void AddValue(List<string> values) =>
            this.AddValue(DefaultLanguageId, values);

        public void AddValue(string languageId, string value) =>
            this.LocalizedValues.Add(new LocalizedValue()
            {
                LanguageId = languageId,
                Value = value
            });

        public void AddValue(string languageId, List<string> values) =>
            this.LocalizedValues.Add(new LocalizedValue()
            {
                LanguageId = languageId,
                Values = values
            });
    }
}