using System.Collections.Generic;
using CcLibrary.AspNetCore.Services;
using CcLibrary.AspNetCore.Services.Abstractions;

namespace CcLibrary.AspNetCore.Common {
    public abstract class MappingProfile {
        public IList<IPropertyMapping> MappingDictionaries { get; set; } = new List<IPropertyMapping>();

        public void AddMappingDictionary<TSource,TDestination>(IDictionary<string, PropertyMappingValue> mappingDictionary) {
            MappingDictionaries.Add(new PropertyMapping<TSource,TDestination>(mappingDictionary));
        }
    }
}