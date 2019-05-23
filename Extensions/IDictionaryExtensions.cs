using System.Collections.Generic;
using CcLibrary.AspNetCore.Common;

namespace CcLibrary.AspNetCore.Extensions {
    public static class IDictionaryExtensions {
        public static IDictionary<string, PropertyMappingValue> AddMappingValue(this IDictionary<string, PropertyMappingValue> dictionary, string sourceProperty, bool revert, params string[] destinationProperties) {
            dictionary.Add(sourceProperty, new PropertyMappingValue(destinationProperties, revert));
            return dictionary;
        }
    }
}