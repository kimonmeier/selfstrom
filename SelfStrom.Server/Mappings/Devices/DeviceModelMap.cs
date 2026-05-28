using AutoMapper;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Server.Models;
using SelfStrom.Shared.Mapping;
using SelfStrom.Shared.Services.Worker;

namespace SelfStrom.Server.Mappings.Devices;

public class DeviceModelMap : IMap<Device, DeviceModel>, IMap<AvailableDevice, DeviceModel>, IMap<DeviceModel, ConnectedDeviceProto>
{
    public void Mapping(IMappingExpression<Device, DeviceModel> mapping)
    {
        mapping
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MacAddress, opt => opt.MapFrom(src => src.MacAddress))
            .ForMember(dest => dest.IPAddress, opt => opt.Ignore());
    }

    public void Mapping(IMappingExpression<AvailableDevice, DeviceModel> mapping)
    {
        mapping
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.MacAddress, opt => opt.MapFrom(src => src.MacAddress))
            .ForMember(dest => dest.IPAddress, opt => opt.MapFrom(src => src.IpAddress));
    }

    public void Mapping(IMappingExpression<DeviceModel, ConnectedDeviceProto> mapping)
    {
        mapping
            .ForMember(dest => dest.DeviceId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.MacAddress, opt => opt.MapFrom(src => src.MacAddress));
    }
}
