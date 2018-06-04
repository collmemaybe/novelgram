namespace Src.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using Src.Models.User;

    [Authorize]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly INovelGramUserClient userClient;

        public UserController(INovelGramUserClient userClient)
        {
            this.userClient = userClient;
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<NovelGramPhotoDetails>> Gallery(string userId)
        {
            var user = string.IsNullOrWhiteSpace(userId) ?
                await this.userClient.GetCurrentUserAsync() :
                await this.userClient.GetUserAsync(userId);

            return user?.PhotoKeys;
        }
    }
}
