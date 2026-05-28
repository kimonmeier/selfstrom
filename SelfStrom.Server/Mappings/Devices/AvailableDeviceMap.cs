using AutoMapper;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Server.Models;
using SelfStrom.Shared.Mapping;
using SelfStrom.Shared.Services.Website;
using SelfStrom.Shared.Services.Worker;

namespace SelfStrom.Server.Mappings.Devices;

public class AvailableDeviceMap : IMap<DeviceModel, AvailableDeviceProto>
{
    public void Mapping(IMappingExpression<DeviceModel, AvailableDeviceProto> mapping)
    {
        mapping
            .ForMember(dest => dest.DeviceId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.IPAddress))
            .ForMember(dest => dest.MacAddress, opt => opt.MapFrom(src => src.MacAddress));
    }
}
