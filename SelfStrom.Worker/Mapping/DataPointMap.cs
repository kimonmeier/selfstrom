using AutoMapper;
using SelfStrom.Shared.Mapping;
using SelfStrom.Shared.Services.Worker;
using SelfStrom.Worker.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStrom.Worker.Mapping;

internal class DataPointMap : IMap<DataPoint, DataPointProto>
{
    public void Mapping(IMappingExpression<DataPoint, DataPointProto> mapping)
    {
        mapping
            .ForMember(dest => dest.DeviceId, opt => opt.MapFrom(src => src.DeviceId))
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value));
    }
}
