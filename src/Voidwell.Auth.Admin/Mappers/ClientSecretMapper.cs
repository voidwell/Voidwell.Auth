using Voidwell.Auth.Admin.Dtos;
using Voidwell.Auth.Data.Entities;

namespace Voidwell.Auth.Admin.Mappers;

internal static class ClientSecretMapper
{
    public static ClientSecretDto ToDto(this ClientSecret entity)
    {
        return new ClientSecretDto
        {
            Id = entity.Id,
            Created = entity.Created,
            Description = entity.Description,
            Expiration = entity.Expiration,
            Value = ObfuscateValue(entity.Value)
        };
    }

    private static string ObfuscateValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "****";
        }

        if (value.Length <= 4)
        {
            return new string('*', value.Length);
        }

        return new string('*', value.Length - 4) + value[^4..];
    }
}
