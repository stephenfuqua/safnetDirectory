using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using safnetDirectory.FullMvc.Models;
using System.Threading.Tasks;

namespace safnetDirectory.FullMvc.Identity
{
    public interface IApplicationSignInManager
    {
        Task SignInAsync(ApplicationUser user, bool isPersistent, bool rememberBrowser);
        Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout);
        Task<bool> HasBeenVerifiedAsync();
        Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser);
        Task<string> GetVerifiedUserIdAsync();
        Task<bool> SendTwoFactorCodeAsync(string provider);
        Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent);
        Task<System.Security.Claims.ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user);
        UserManager<ApplicationUser, string> UserManager { get; set; }
    }
}