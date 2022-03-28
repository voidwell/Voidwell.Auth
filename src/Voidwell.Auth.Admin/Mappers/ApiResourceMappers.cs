using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Voidwell.Auth.Admin.Models;

namespace Voidwell.Auth.Admin.Mappers
{
    public static class ApiResourceMappers
    {
        static ApiResourceMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiResourceMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static ApiResourceApiDto ToModel(this ApiResource resource)
        {
            return resource == null ? null : Mapper.Map<ApiResourceApiDto>(resource);
        }

        public static SecretApiDto ToModel(this ApiSecret apiSecret)
        {
            return Mapper.Map<SecretApiDto>(apiSecret);
        }

        public static ApiResource ToEntity(this ApiResourceApiDto resource)
        {
            if (resource == null)
            {
                return null;
            }

            var mapResource = Mapper.Map<ApiResource>(resource);
            mapResource.Properties?.ForEach(a => a.ApiResourceId = resource.Id);
            mapResource.Scopes?.ForEach(a => a.ApiResourceId = resource.Id);
            mapResource.UserClaims?.ForEach(a => a.ApiResourceId = resource.Id);
            return mapResource;
        }
    }
}
