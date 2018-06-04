namespace Src.Models.Photo
{
    using System.Threading.Tasks;

    public interface INovelGramPhotoFactory
    {
        Task<NovelGramPhoto> NewPhotoAsync();
    }
}