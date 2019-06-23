using AutoHateoas.AspNetCore.Common;
using AutoHateoas.AspNetCore.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoHateoas.AspNetCore.Services {
    public class PropertyMappingService : IPropertyMappingService {
        private IList<IPropertyMapping> mappingDictionaries;

        public PropertyMappingService(MappingProfile values) {
            mappingDictionaries = values.MappingDictionaries;
        }

        public IDictionary<string, PropertyMappingValue> GetMapping<TSource, TDestination>() {
            var matchingMapping = mappingDictionaries.OfType<PropertyMapping<TSource, TDestination>>();
            if (matchingMapping.Count() == 1) {
                return matchingMapping.First().PropertyMappingDictionary;
            }
            throw new ArgumentException($"No se pudo realizar el Mapeo de los tipos {typeof(TSource)} a {typeof(TDestination)}");
        }

        public bool ValidateMappingsForOrderBy<TSource, TDestination>(string orderBy) {
            bool isValid = true;
            var mappigDictionary = GetMapping<TSource, TDestination>();
            if (string.IsNullOrEmpty(orderBy)) return true;
            var orderByAfterSplit = orderBy.Split(',');
            foreach (var splitedOrderBy in orderByAfterSplit) {
                var trimmedOrderBy = splitedOrderBy.Trim();
                int indexOfSpace = trimmedOrderBy.IndexOf(" ");
                string actualOrderBy = trimmedOrderBy;
                if (indexOfSpace != -1) {
                    actualOrderBy = trimmedOrderBy.Substring(0, indexOfSpace);
                }
                isValid &= mappigDictionary.ContainsKey(actualOrderBy);
            }
            return isValid;
        }
    }
}