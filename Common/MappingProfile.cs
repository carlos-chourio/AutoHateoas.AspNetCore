using System.Collections.Generic;
using AutoHateoas.AspNetCore.Services;
using AutoHateoas.AspNetCore.Services.Abstractions;

namespace AutoHateoas.AspNetCore.Common {
    public abstract class MappingProfile {
        public IList<IPropertyMapping> MappingDictionaries { get; set; } = new List<IPropertyMapping>();

        public void AddMappingDictionary<TSource,TDestination>(IDictionary<string, PropertyMappingValue> mappingDictionary) {
            MappingDictionaries.Add(new PropertyMapping<TSource,TDestination>(mappingDictionary));
        }
    }
}