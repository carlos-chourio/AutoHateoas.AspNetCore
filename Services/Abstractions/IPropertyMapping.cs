using CcLibrary.AspNetCore.Common;
using System.Collections.Generic;

namespace CcLibrary.AspNetCore.Services.Abstractions {
    public class IPropertyMapping {
        IDictionary<string, PropertyMappingValue> PropertyMappingDictionary { get; }
    }
}