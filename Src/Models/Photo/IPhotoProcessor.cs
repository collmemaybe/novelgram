namespace Src.Models.Photo
{
    using System.IO;

    public interface IPhotoProcessor
    {
        Stream ProcessThumbnail(Stream photoData);
    }
}