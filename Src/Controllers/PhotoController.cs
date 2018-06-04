namespace Src.Controllers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Src.Models.Photo;
    using Src.Models.User;

    [Authorize]
    public class PhotoController : Controller
    {
        private readonly INovelGramUserClient userClient;

        private readonly IPhotoService photoService;

        public PhotoController(IPhotoService photoService)
        {
            this.userClient = userClient ?? throw new ArgumentNullException(nameof(userClient));
            this.photoService = photoService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile formFile)
        {
            if (formFile.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream);
                    await this.photoService.AddPhotoAsync(stream);
                }
            }
            
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Download(string userId, string key, bool isThumbnail)
        {
            using (var stream = await this.photoService.GetPhotoAsync(userId, key, isThumbnail))
            {
                // todo: handle other formats
                return this.File(stream, "image/jpeg");
            }
        }
    }
}
