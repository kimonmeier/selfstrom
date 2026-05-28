using AutoMapper;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Shared.Mapping;
using SelfStrom.Shared.Services.Website;

namespace SelfStrom.Server.Mappings.Devices;

public class DeviceMap : IMap<Device, DeviceProto>
{
    public void Mapping(IMappingExpression<Device, DeviceProto> mapping)
    {
        mapping
            .ForMember(dest => dest.DeviceId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MacAddress, opt => opt.MapFrom(src => src.MacAddress))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.LastUpdate, opt => opt.MapFrom(src => src.LastSeen));
    }
}
