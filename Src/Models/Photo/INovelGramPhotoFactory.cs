namespace Src.Models.Photo
{
    using System.Threading.Tasks;

    using Src.Models.User;

    public interface INovelGramPhotoFactory
    {
        Task<NovelGramPhoto> NewPhotoAsync();

        NovelGramPhoto BuildPhoto(string userId, NovelGramPhotoDetails details);
    }
}