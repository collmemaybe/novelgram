namespace Src.Models.User
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface INovelGramUserClient
    {
        Task<IList<NovelGramUser>> GetUserBatchAsync(IList<string> userIds);

        Task<NovelGramUser> GetUserAsync(string userId);

        Task SaveUserAsync(NovelGramUser user);

        Task<NovelGramUser> GetCurrentUserAsync();
    }
}