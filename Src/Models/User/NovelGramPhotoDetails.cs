namespace Src.Models.User
{
    using Newtonsoft.Json;

    [JsonObject]
    public class NovelGramPhotoDetails
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }
    }
}