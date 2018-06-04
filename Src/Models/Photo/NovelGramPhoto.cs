namespace Src.Models.Photo
{
    using System;

    public class NovelGramPhoto
    {
        public NovelGramPhoto(
            string root,
            string userId,
            string key,
            DateTimeOffset timestamp)
        {
            this.UserId = userId;
            this.Key = $"{root}-{userId.Replace("@", string.Empty)}-{key}";
            this.Timestamp = timestamp;
        }

        public string UserId { get; }
        
        public string ThumbnailKey => this.Key + "-t";

        public string Key { get; }

        public DateTimeOffset Timestamp { get; }
    }
}