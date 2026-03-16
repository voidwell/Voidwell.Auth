using OpenIddict.EntityFrameworkCore.Models;

namespace Voidwell.Auth.Data.Entities;

public class AuthAuthorization : OpenIddictEntityFrameworkCoreAuthorization<int, AuthApplication, AuthToken>
{
}
