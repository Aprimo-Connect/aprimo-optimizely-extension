using Newtonsoft.Json;

namespace Aprimo.Opti.Models.RestImpl
{
    public partial class Metadata
    {
        [JsonProperty("isCroppedImage", NullValueHandling = NullValueHandling.Ignore)]
        public long? IsCroppedImage { get; set; }

        [JsonProperty("x", NullValueHandling = NullValueHandling.Ignore)]
        public long? X { get; set; }

        [JsonProperty("y", NullValueHandling = NullValueHandling.Ignore)]
        public long? Y { get; set; }

        [JsonProperty("cropWidth", NullValueHandling = NullValueHandling.Ignore)]
        public long? CropWidth { get; set; }

        [JsonProperty("cropHeight", NullValueHandling = NullValueHandling.Ignore)]
        public long? CropHeight { get; set; }

        [JsonProperty("resizeWidth", NullValueHandling = NullValueHandling.Ignore)]
        public long? ResizeWidth { get; set; }

        [JsonProperty("resizeHeight", NullValueHandling = NullValueHandling.Ignore)]
        public long? ResizeHeight { get; set; }

        [JsonProperty("resolution", NullValueHandling = NullValueHandling.Ignore)]
        public long? Resolution { get; set; }

        [JsonProperty("colorspace", NullValueHandling = NullValueHandling.Ignore)]
        public string Colorspace { get; set; }

        [JsonProperty("resizeFormat", NullValueHandling = NullValueHandling.Ignore)]
        public string ResizeFormat { get; set; }

        [JsonProperty("presetName", NullValueHandling = NullValueHandling.Ignore)]
        public string PresetName { get; set; }

        [JsonProperty("presetHash", NullValueHandling = NullValueHandling.Ignore)]
        public string PresetHash { get; set; }

        [JsonProperty("isSmart", NullValueHandling = NullValueHandling.Ignore)]
        public long? IsSmart { get; set; }

        [JsonProperty("isPreset", NullValueHandling = NullValueHandling.Ignore)]
        public long? IsPreset { get; set; }

        [JsonProperty("cropPreviewAdditionalFileId", NullValueHandling = NullValueHandling.Ignore)]
        public string CropPreviewAdditionalFileId { get; set; }

        [JsonProperty("isCropPreview", NullValueHandling = NullValueHandling.Ignore)]
        public long? IsCropPreview { get; set; }

        [JsonProperty("cropAdditionalFileId", NullValueHandling = NullValueHandling.Ignore)]
        public string CropAdditionalFileId { get; set; }
    }
}