namespace Src.Models.User
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;

    public class CurrentUserManager : ICurrentUserManager
    {
        private readonly UserManager<ApplicationUser> userManager;

        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserManager(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            var principal = this.httpContextAccessor.HttpContext.User;
            var user = await this.userManager.GetUserAsync(principal);

            if (user?.Email == null)
            {
                throw new InvalidOperationException("No user present in the current context or no user id");
            }

            return user.Email;
        }
    }
}
