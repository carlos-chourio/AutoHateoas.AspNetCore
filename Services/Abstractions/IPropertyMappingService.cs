using CcLibrary.AspNetCore.Common;
using System.Collections.Generic;

namespace CcLibrary.AspNetCore.Services.Abstractions {
    
    public interface IPropertyMappingService {
        bool ValidateMappingsForOrderBy<TSource, TDestination>(string orderBy);
        IDictionary<string, PropertyMappingValue> GetMapping<TSource, TDestination>();
    }
}