namespace Src.Models.User
{
    using System.Threading.Tasks;

    public interface ICurrentUserManager
    {
        Task<string> GetCurrentUserIdAsync();
    }
}