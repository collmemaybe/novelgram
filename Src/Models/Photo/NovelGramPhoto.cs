﻿namespace Src.Models.Photo
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
            this.Bucket = $"novelgram/{root}-{userId.Replace("@", string.Empty)}";
            this.Key = key;
            this.Timestamp = timestamp;
        }

        public string UserId { get; }

        public string Bucket { get; }

        public string ThumbnailKey => this.FullId + "-t";

        public string Key { get; }

        public DateTimeOffset Timestamp { get; }

        public string FullId => $"{this.Bucket}-{this.Key}";
    }
}