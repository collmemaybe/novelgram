namespace Src.Models.Photo
{
    using System.IO;

    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats.Jpeg;
    using SixLabors.ImageSharp.Processing;
    using SixLabors.ImageSharp.Processing.Transforms;
    using SixLabors.Primitives;

    public class PhotoProcessor : IPhotoProcessor
    {
        private static readonly Size ThumbnailSize = new Size(150, 150);

        public Stream ProcessThumbnail(Stream photoData)
        {
            var image = Image.Load(photoData);
            var imageSize = image.Size();

            ResizeMode mode = imageSize.Height < ThumbnailSize.Height || imageSize.Width < ThumbnailSize.Width ? ResizeMode.Crop : ResizeMode.Stretch;

            var resizeOptions = new ResizeOptions
            {
                Mode = mode,
                Size = ThumbnailSize,
                Position = AnchorPositionMode.Center
            };

            image.Mutate(i => i.AutoOrient().Resize(resizeOptions));

            var thumnailStream = new MemoryStream();
            image.Save(thumnailStream, new JpegEncoder());
            return thumnailStream;
        }
    }
}
