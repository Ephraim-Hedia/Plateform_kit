using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Platform.Infrastructure.Identity;

namespace Platform.UnitTests.Infrastructure.Identity.TestDoubles;

internal static class IdentityTestHelpers
{
    public static UserManager<ApplicationUser> CreateUserManager()
    {
        var store = Substitute.For<IUserStore<ApplicationUser>>();

        return Substitute.For<UserManager<ApplicationUser>>(
            store, null, null, null, null, null, null, null, null);
    }

    public static SignInManager<ApplicationUser> CreateSignInManager(UserManager<ApplicationUser> userManager)
    {
        var contextAccessor = Substitute.For<IHttpContextAccessor>();
        var claimsFactory = Substitute.For<IUserClaimsPrincipalFactory<ApplicationUser>>();

        return Substitute.For<SignInManager<ApplicationUser>>(
            userManager, contextAccessor, claimsFactory, null, null, null, null);
    }
}
