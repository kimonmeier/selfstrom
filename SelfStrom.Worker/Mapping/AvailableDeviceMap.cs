using AutoMapper;
using MyStrom.Api.Models.DeviceInfo;
using SelfStrom.Shared.Mapping;
using SelfStrom.Shared.Services.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStrom.Worker.Mapping;

public class AvailableDeviceMap : IMap<DeviceInfoResponse, AvailableDevice>
{
    public void Mapping(IMappingExpression<DeviceInfoResponse, AvailableDevice> mapping)
    {
        mapping
            .ForMember(dest => dest.MacAddress, opt => opt.MapFrom(src => src.MacAddress))
            .ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.IPAddress));
    }
}
