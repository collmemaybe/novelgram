namespace Src.Models.User
{
    using System;
    using System.Threading.Tasks;

    using Src.Models.Photo;

    public class NovelGramUserService
    {
        private readonly INovelGramUserClient userClient;

        public NovelGramUserService(INovelGramUserClient userClient)
        {
            this.userClient = userClient ?? throw new ArgumentNullException(nameof(userClient));
        }

        public async Task AddPhoto(NovelGramPhoto photo)
        {
            var currentUser = await this.userClient.GetCurrentUserAsync();
            throw new NotImplementedException();
        }
    }
}