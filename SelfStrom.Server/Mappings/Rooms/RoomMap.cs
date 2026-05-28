using AutoMapper;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Shared.Mapping;
using SelfStrom.Shared.Services.Website;

namespace SelfStrom.Server.Mappings.Rooms;

public class RoomMap : IMap<Room, RoomProto>, IMap<RoomProto, Room>
{
    public void Mapping(IMappingExpression<RoomProto, Room> mapping)
    {
        mapping
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Number, opt => opt.Ignore())
            .ForMember(dest => dest.Home, opt => opt.Ignore())
            .ForMember(dest => dest.HomeId, opt => opt.Ignore())
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
    }

    public void Mapping(IMappingExpression<Room, RoomProto> mapping)
    {
        mapping
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
    }
}
