namespace Src.Models.User
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using Src.Models.Photo;

    [JsonObject]
    public class NovelGramUser
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("photos")]
        public IList<NovelGramPhoto> Photos { get; set; }
    }
}