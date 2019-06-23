using System;
using System.Linq;
using System.Reflection;

namespace AutoHateoas.AspNetCore.Validation {
    public static class PropertiesValidator {
        public static bool ValidatePropertiesExist(Type type, string fieldsSeparatedByCommas) {
            if (!string.IsNullOrEmpty(fieldsSeparatedByCommas)) {
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
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