using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthServer.Config
{
    public class PhoneNumberConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
    {
        public PhoneNumberConfirmationTokenProvider(
            IDataProtectionProvider dataProtectionProvider,
            IOptions<PhoneNumberConfirmationTokenProviderOptions> options,
            ILogger<DataProtectorTokenProvider<TUser>> logger) : base(dataProtectionProvider, options, logger)
        {
        }
    }
    public class PhoneNumberConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
    {
    }
}
