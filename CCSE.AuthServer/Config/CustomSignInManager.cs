using AuthServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace AuthServer.Config
{
    /// <summary>
    /// Custom class to enable either email or phone number
    /// </summary>
    public class CustomSignInManager : SignInManager<AppUser>
    {
        public CustomSignInManager(
            UserManager<AppUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<AppUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<AppUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<AppUser> confirmation) : base(
                userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        public override async Task<bool> CanSignInAsync(AppUser user)
        {
            var emailConfirmed = await UserManager.IsEmailConfirmedAsync(user);
            var phoneConfirmed = await UserManager.IsPhoneNumberConfirmedAsync(user);

            return emailConfirmed || phoneConfirmed;
        }
    }
}
