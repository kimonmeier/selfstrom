using AutoMapper;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Shared.Mapping;
using SelfStrom.Shared.Services.Website;

namespace SelfStrom.Server.Mappings.Workers;

public class WorkerMap : IMap<Worker, WorkerProto>
{
    public void Mapping(IMappingExpression<Worker, WorkerProto> mapping)
    {
        mapping
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.WorkerName))
            .ForMember(dest => dest.HomeId, opt => opt.MapFrom(src => src.HomeId))
            .ForMember(dest => dest.HomeName, opt => opt.MapFrom(src => src.Home.Name))
            .ForMember(dest => dest.HomeNumber, opt => opt.MapFrom(src => src.Home.Number))
            .ForMember(dest => dest.Online, opt => opt.MapFrom<WorkerStatusResolver>());
    }
}
