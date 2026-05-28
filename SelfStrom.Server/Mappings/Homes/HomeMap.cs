using AutoMapper;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Shared.Mapping;
using SelfStrom.Shared.Services.Website;

namespace SelfStrom.Server.Mappings.Homes;

public class HomeMap : IMap<Home, HomeProto>, IMap<HomeProto, Home>
{
    public void Mapping(IMappingExpression<Home, HomeProto> mapping)
    {
        mapping
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number));
    }

    public void Mapping(IMappingExpression<HomeProto, Home> mapping)
    {
        mapping
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Number, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.Number, opt => opt.Ignore());
    }
}
