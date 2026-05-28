using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStrom.Worker.Mapping.Converter;

internal class TimeStampConverter : ITypeConverter<DateTime, Timestamp>, ITypeConverter<Timestamp, DateTime>
{
    public Timestamp Convert(DateTime source, Timestamp destination, ResolutionContext context)
    {
        return Timestamp.FromDateTime(source.ToUniversalTime());
    }

    public DateTime Convert(Timestamp source, DateTime destination, ResolutionContext context)
    {
        return source.ToDateTime();
    }
}
