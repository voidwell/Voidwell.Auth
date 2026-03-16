using OpenIddict.EntityFrameworkCore.Models;

namespace Voidwell.Auth.Data.Entities;

public class AuthToken : OpenIddictEntityFrameworkCoreToken<int, AuthApplication, AuthAuthorization>
{
}
