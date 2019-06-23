using AutoHateoas.AspNetCore.Common;
using AutoHateoas.AspNetCore.Services.Abstractions;
using System.Collections.Generic;

namespace AutoHateoas.AspNetCore.Services {
    public class PropertyMapping<TSource,TValue> : IPropertyMapping {
        public IDictionary<string, PropertyMappingValue> PropertyMappingDictionary { get; private set; }
        public PropertyMapping(IDictionary<string,PropertyMappingValue> propertyMappingDictionary) {
            PropertyMappingDictionary = propertyMappingDictionary;
        }
    }
}