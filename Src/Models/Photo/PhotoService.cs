namespace Src.Models.Photo
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Amazon.S3;
    using Amazon.S3.Transfer;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats;
    using SixLabors.ImageSharp.Formats.Jpeg;
    using SixLabors.ImageSharp.Processing;
    using SixLabors.ImageSharp.Processing.Transforms;
    using SixLabors.Primitives;

    using Src.Models.User;

    public class PhotoService : IPhotoService
    {
        private static readonly Size ThumbnailSize = new Size(150, 150);

        private readonly ILogger<PhotoService> logger;

        private readonly INovelGramUserClient userClient;

        private readonly ITransferUtility transferUtility;

        private readonly INovelGramPhotoFactory photoFactory;

        public PhotoService(
            ITransferUtility transferUtility,
            INovelGramPhotoFactory photoFactory, 
            INovelGramUserClient userClient,
            ILogger<PhotoService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.transferUtility = transferUtility ?? throw new ArgumentNullException(nameof(transferUtility));
            this.photoFactory = photoFactory ?? throw new ArgumentNullException(nameof(photoFactory));
            this.userClient = userClient ?? throw new ArgumentNullException(nameof(userClient));
        }

        public async Task<Stream> GetPhotoAsync(string userId, string key, bool isThumbnail)
        {
            var user = await this.userClient.GetUserAsync(userId);

            if (user?.PhotoKeys == null || !user.PhotoKeys.Any())
            {
                return null;
            }

            var details = user.PhotoKeys.FirstOrDefault(d => string.Equals(d.Key, key, StringComparison.OrdinalIgnoreCase));
            var photo = this.photoFactory.BuildPhoto(userId, details);

            var downloadRequest = new TransferUtilityOpenStreamRequest
            {
                BucketName = photo.Bucket,
                Key = GetKey(photo, isThumbnail)
            };

            return await this.transferUtility.OpenStreamAsync(downloadRequest);
        }

        public async Task AddPhotoAsync(Stream photo)
        {
            var newDetailsTask = this.photoFactory.NewPhotoAsync();
            var currentUserTask = this.userClient.GetCurrentUserAsync();

            await Task.WhenAll(newDetailsTask, currentUserTask);

            var currentUser = await currentUserTask;

            currentUser.PhotoKeys = currentUser.PhotoKeys == null ? 
                new List<NovelGramPhotoDetails>() :
                new List<NovelGramPhotoDetails>(currentUser.PhotoKeys);

            var newDetails = await newDetailsTask;
            var thumbnail = ProcessThumbnail(photo, out var format);

            var details = new NovelGramPhotoDetails
            {
                Format = format.Name,
                Key = newDetails.Key,
                Timestamp = newDetails.Timestamp.ToUnixTimeSeconds()
            };

            currentUser.PhotoKeys.Add(details);

            var uploadThumbnailTask = this.UploadToS3(thumbnail, newDetails, isThumbnail: true);
            var uploadPhotoTask = this.UploadToS3(photo, newDetails, isThumbnail: false);
            var saveUserTask = this.userClient.SaveUserAsync(currentUser);

            await Task.WhenAll(uploadPhotoTask, uploadThumbnailTask, saveUserTask);
        }

        private static string GetKey(NovelGramPhoto photo, bool isThumbnail) => isThumbnail ? photo.ThumbnailKey : photo.Key;

        private async Task UploadToS3(Stream data, NovelGramPhoto photo, bool isThumbnail)
        {
            try
            {
                string key = GetKey(photo, isThumbnail);
                await this.transferUtility.UploadAsync(data, photo.Bucket, key, CancellationToken.None);
                this.logger.LogInformation($"Uploaded photo {photo.FullId}");
            }
            catch (AmazonS3Exception s3Exception)
            {
                this.logger.LogError(s3Exception, "Error uploading photo {0}", "");
            }
        }

        private static Stream ProcessThumbnail(Stream photoData, out IImageFormat format)
        {
            var image = Image.Load(photoData, out format);
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
