namespace Src.Models.User
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    [JsonObject]
    public class NovelGramUser
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("photos")]
        public IList<NovelGramPhotoDetails> PhotoKeys { get; set; }

        [JsonProperty("friends")]
        public IList<string> Friends { get; set; }
    }
}