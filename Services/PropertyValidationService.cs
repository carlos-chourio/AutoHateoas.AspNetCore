using CcLibrary.AspNetCore.Services.Abstractions;
using System;
using System.Linq;
using System.Reflection;

namespace CcLibrary.AspNetCore.Services {
    public class PropertyValidationService : IPropertyValidationService {
        public bool ValidateProperties<TType>(string fieldsSeparatedByCommas) {
            if (!string.IsNullOrEmpty(fieldsSeparatedByCommas)) {
                var properties = typeof(TType).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var fieldsAfterSplit = fieldsSeparatedByCommas.Split(',');
                foreach (var field in fieldsAfterSplit) {
                    if (!properties.Any(t=> t.Name.Equals(field.Trim(), StringComparison.CurrentCultureIgnoreCase))){
                        return false;
                    }
                }
            }
            return true;
        }
    }
}