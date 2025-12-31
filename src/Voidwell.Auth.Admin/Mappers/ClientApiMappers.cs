using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Voidwell.Auth.Admin.Models;

namespace Voidwell.Auth.Admin.Mappers;

public static class ClientApiMappers
{
    static ClientApiMappers()
    {
        Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientApiMapperProfile>())
            .CreateMapper();
    }

    internal static IMapper Mapper { get; }

    public static ClientApiDto ToModel(this Client client)
    {
        return Mapper.Map<ClientApiDto>(client);
    }

    public static SecretApiDto ToModel(this ClientSecret clientSecret)
    {
        return Mapper.Map<SecretApiDto>(clientSecret);
    }

    public static Client ToEntity(this ClientApiDto client)
    {
        var mapClient = Mapper.Map<Client>(client);
        mapClient.AllowedGrantTypes?.ForEach(a => a.ClientId = client.Id);
        mapClient.AllowedCorsOrigins?.ForEach(a => a.ClientId = client.Id);
        mapClient.AllowedScopes?.ForEach(a => a.ClientId = client.Id);
        mapClient.Claims?.ForEach(a => a.ClientId = client.Id);
        mapClient.IdentityProviderRestrictions?.ForEach(a => a.ClientId = client.Id);
        mapClient.PostLogoutRedirectUris?.ForEach(a => a.ClientId = client.Id);
        mapClient.Properties?.ForEach(a => a.ClientId = client.Id);
        mapClient.RedirectUris?.ForEach(a => a.ClientId = client.Id);
        return mapClient;
    }
}
