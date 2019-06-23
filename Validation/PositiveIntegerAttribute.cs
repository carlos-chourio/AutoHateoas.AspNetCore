using System.ComponentModel.DataAnnotations;

namespace AutoHateoas.AspNetCore.Validation {
    public class PositiveIntegerAttribute : ValidationAttribute {
        public override bool IsValid(object value) {
            if (int.TryParse(value.ToString(), out int x)) {
                return x > 0;
            }
            return false;
        }
    }
}
