using System.Linq;
using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Voidwell.Auth.Admin.Models;

namespace Voidwell.Auth.Admin.Mappers
{
    public class ApiResourceMapperProfile : Profile
    {
        public ApiResourceMapperProfile()
        {
            // entity to model
            CreateMap<ApiResource, ApiResourceApiDto>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => x.Type)));

            CreateMap<ApiScope, ApiScopeApiDto>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opt => opt.MapFrom(src => src.UserClaims.Select(x => x.Type)));

            CreateMap<ApiSecret, ApiSecretApiDto>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null));

            CreateMap<ApiResourceProperty, ApiResourcePropertyApiDto>(MemberList.Destination)
                .ReverseMap();

            // model to entity
            CreateMap<ApiResourceApiDto, ApiResource>(MemberList.Source)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiResourceClaim { Type = x })));

            CreateMap<ApiScopeApiDto, ApiScope>(MemberList.Source)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiScopeClaim { Type = x })));
        }
    }
}
