using AutoMapper;
using System.Collections;
using System.Collections.Generic;

namespace Zero.AutoMapper
{
    public static class AutoMapExtensions
    {
        public static TDestination MapTo<TDestination>(this object source)
        {
            if (source == null)
            {
                return default(TDestination);
            }
            Mapper.Initialize(init =>
            {
                init.CreateMap(source.GetType(), typeof(TDestination));
            });
            return Mapper.Map<TDestination>(source);
        }
        public static List<TDestination> MapTo<TDestination>(this IEnumerable source)
        {
            if (source == null)
            {
                return new List<TDestination>();
            }
            foreach (var first in source)
            {
                var type = first.GetType();
                Mapper.Initialize(init =>
                {
                    init.CreateMap(type, typeof(TDestination));
                });
                break;
            }
            return Mapper.Map<List<TDestination>>(source);
        }
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            Mapper.Initialize(init =>
            {
                init.CreateMap<TSource, TDestination>();
            });
            return Mapper.Map(source, destination);
        }
    }
}
