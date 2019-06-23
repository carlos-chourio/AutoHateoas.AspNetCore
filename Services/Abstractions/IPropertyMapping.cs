using AutoHateoas.AspNetCore.Common;
using System.Collections.Generic;

namespace AutoHateoas.AspNetCore.Services.Abstractions {
    public class IPropertyMapping {
        IDictionary<string, PropertyMappingValue> PropertyMappingDictionary { get; }
    }
}