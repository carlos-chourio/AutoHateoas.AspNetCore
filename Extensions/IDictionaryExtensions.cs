using System.Collections.Generic;
using AutoHateoas.AspNetCore.Common;

namespace AutoHateoas.AspNetCore.Extensions {
    public static class IDictionaryExtensions {
        public static IDictionary<string, PropertyMappingValue> AddMappingValue(this IDictionary<string, PropertyMappingValue> dictionary, string sourceProperty, bool revert, params string[] destinationProperties) {
            dictionary.Add(sourceProperty, new PropertyMappingValue(destinationProperties, revert));
            return dictionary;
        }
    }
}