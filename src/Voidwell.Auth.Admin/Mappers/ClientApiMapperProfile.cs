using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using Voidwell.Auth.Admin.Models;

namespace Voidwell.Auth.Admin.Mappers;

public class ClientApiMapperProfile : Profile
{
    public ClientApiMapperProfile()
    {
        // entity to model
        CreateMap<Client, ClientApiDto>(MemberList.Destination)
            .ForMember(dest => dest.ProtocolType, opt => opt.Condition(srs => srs != null))
            .ReverseMap();

        CreateMap<ClientGrantType, string>()
            .ConstructUsing(src => src.GrantType)
            .ReverseMap()
            .ForMember(dest => dest.GrantType, opt => opt.MapFrom(src => src));

        CreateMap<ClientRedirectUri, string>()
            .ConstructUsing(src => src.RedirectUri)
            .ReverseMap()
            .ForMember(dest => dest.RedirectUri, opt => opt.MapFrom(src => src));

        CreateMap<ClientPostLogoutRedirectUri, string>()
            .ConstructUsing(src => src.PostLogoutRedirectUri)
            .ReverseMap()
            .ForMember(dest => dest.PostLogoutRedirectUri, opt => opt.MapFrom(src => src));

        CreateMap<ClientScope, string>()
            .ConstructUsing(src => src.Scope)
            .ReverseMap()
            .ForMember(dest => dest.Scope, opt => opt.MapFrom(src => src));

        CreateMap<ClientSecret, SecretApiDto>(MemberList.Destination)
            .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
            .ReverseMap();

        CreateMap<ClientClaim, ClientClaimApiDto>(MemberList.Destination)
            .ConstructUsing(src => new ClientClaimApiDto() { Type = src.Type, Value = src.Value })
            .ReverseMap();

        CreateMap<ClientIdPRestriction, string>()
            .ConstructUsing(src => src.Provider)
            .ReverseMap()
            .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src));

        CreateMap<ClientCorsOrigin, string>()
            .ConstructUsing(src => src.Origin)
            .ReverseMap()
            .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src));

        CreateMap<ClientProperty, ClientPropertyApiDto>(MemberList.Destination)
            .ReverseMap();

        // model to entity
        CreateMap<SecretApiDto, ClientSecret>(MemberList.Source)
            .ForMember(x => x.Id, opt => opt.MapFrom(src => src.Id));

        CreateMap<ClientClaimApiDto, ClientClaim>(MemberList.Source)
            .ForMember(x => x.Id, opt => opt.MapFrom(src => src.Id));

        CreateMap<ClientPropertyApiDto, ClientProperty>(MemberList.Source)
            .ForMember(x => x.Id, opt => opt.MapFrom(src => src.Id));
    }
}
