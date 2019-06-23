using AutoHateoas.AspNetCore.Common;
using System.Collections.Generic;

namespace AutoHateoas.AspNetCore.Services.Abstractions {
    
    public interface IPropertyMappingService {
        bool ValidateMappingsForOrderBy<TSource, TDestination>(string orderBy);
        IDictionary<string, PropertyMappingValue> GetMapping<TSource, TDestination>();
    }
}