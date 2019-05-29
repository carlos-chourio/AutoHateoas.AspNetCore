using System;
using System.ComponentModel.DataAnnotations;

namespace CcLibrary.AspNetCore.Validation {
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertiesExistAttribute : ValidationAttribute {
        public string TypeName { get; }

        public PropertiesExistAttribute(string type) {
            TypeName = type;
        }

        public override bool IsValid(object value) {
            string stringValue = value as string;
            if (stringValue!=null) {
                Type type = Type.GetType(TypeName);
                return PropertiesValidator.ValidatePropertiesExist(type, stringValue);
            }
            return false;
        }

    }
}