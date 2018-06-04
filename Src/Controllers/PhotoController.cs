namespace Src.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;

    using Amazon.Runtime.Internal.Util;
    using Amazon.S3;
    using Amazon.S3.Transfer;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Src.Extensions;
    using Src.Models.Photo;
    using Src.Models.User;

    [Authorize]
    public class PhotoController : Controller
    {
        private readonly ITransferUtility transferUtility;

        private readonly INovelGramPhotoFactory photoFactory;

        private readonly INovelGramUserClient userClient;

        private readonly ILogger<PhotoController> logger;

        public PhotoController(
            ITransferUtility transferUtility,
            INovelGramPhotoFactory photoFactory,
            INovelGramUserClient userClient,
            ILogger<PhotoController> logger)
        {
            this.transferUtility = transferUtility;
            this.photoFactory = photoFactory;
            this.userClient = userClient ?? throw new ArgumentNullException(nameof(userClient));
            this.logger = logger;
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            var filePath = Path.GetTempFileName();

            foreach (var formFile in files.Where(f => f.Length > 0))
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }

                    await this.UploadToS3(formFile);
                }
            }
            
            return Ok();
        }

        private async Task UploadToS3(IFormFile file)
        {
            try
            {
                var photo = await this.photoFactory.NewPhotoAsync();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    await this.transferUtility.UploadAsync(stream, photo.Bucket, photo.Key, CancellationToken.None);
                    // await this.userClient.AddPhotoAsync(photo);
                }

                this.logger.LogInformation($"Uploaded photo {photo.FullId}");
            }
            catch (AmazonS3Exception s3Exception)
            {
                this.logger.LogError(s3Exception, "Error uploading photo {0}", "");
            }
        }
    }
}
