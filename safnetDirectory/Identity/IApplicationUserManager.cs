using Microsoft.AspNet.Identity;
using safnetDirectory.FullMvc.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace safnetDirectory.FullMvc.Identity
{
    public interface IApplicationUserManager
    {
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<ApplicationUser> FindByIdAsync(string userId);
        Task<string> GenerateTwoFactorTokenAsync(string userId, string twoFactorProvider);
        Task<IdentityResult> ConfirmEmailAsync(string userId, string token);
        Task<ApplicationUser> FindByNameAsync(string userName);
        Task<bool> IsEmailConfirmedAsync(string userId);
        Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword);
        Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId);
        Task<IdentityResult> CreateAsync(ApplicationUser user);
        Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login);
        Task<IdentityResult> AddToRoleAsync(string userId, string role);
    }
}
