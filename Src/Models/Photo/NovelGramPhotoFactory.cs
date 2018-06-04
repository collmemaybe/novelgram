namespace Src.Models.Photo
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Configuration;

    using Src.Models.User;

    public class NovelGramPhotoFactory : INovelGramPhotoFactory
    {
        private const string BucketNameKey = "Photo:BucketName";

        private readonly string root;

        private readonly ICurrentUserManager currentUserManager;

        public NovelGramPhotoFactory(
            IConfiguration configuration,
            ICurrentUserManager currentUserManager)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            this.currentUserManager = currentUserManager ?? throw new ArgumentNullException(nameof(currentUserManager));

            this.root = configuration[BucketNameKey];

            if (string.IsNullOrWhiteSpace(this.root))
            {
                throw new ArgumentException($"Missing key  {BucketNameKey}", nameof(configuration));
            }
        }

        public NovelGramPhoto BuildPhoto(string userId, NovelGramPhotoDetails details)
        {
            var timestamp = DateTimeOffset.FromUnixTimeSeconds(details.Timestamp);
            return new NovelGramPhoto(this.root, userId, details.Key, timestamp);
        }

        public async Task<NovelGramPhoto> NewPhotoAsync()
        {
            string key = Guid.NewGuid().ToString();
            string userId = await this.currentUserManager.GetCurrentUserIdAsync();

            return new NovelGramPhoto(this.root, userId, key, DateTimeOffset.UtcNow);
        }
    }
}