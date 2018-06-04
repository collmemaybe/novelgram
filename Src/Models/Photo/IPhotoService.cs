namespace Src.Models.Photo
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IPhotoService
    {
        Task AddPhotoAsync(Stream photo);

        Task<Stream> GetPhotoAsync(string userId, string key, bool isThumbnail);
    }
}