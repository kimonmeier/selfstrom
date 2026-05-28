using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using SelfStrom.Shared.Mapping;
using SelfStrom.Worker.Mapping.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStrom.Worker.Mapping;

internal class TimeStampMap : IMap<Timestamp, DateTime>, IMap<DateTime, Timestamp>
{
    public void Mapping(IMappingExpression<DateTime, Timestamp> mapping)
    {
        mapping
            .ConvertUsing<TimeStampConverter>();
    }

    public void Mapping(IMappingExpression<Timestamp, DateTime> mapping)
    {
        mapping
            .ConvertUsing<TimeStampConverter>();
    }
}
