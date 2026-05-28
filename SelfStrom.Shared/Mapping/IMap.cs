using AutoMapper;

namespace SelfStrom.Shared.Mapping;

public interface IMap<TSource, TDestination>
{
    public void Mapping(IMappingExpression<TSource, TDestination> mapping);
}
