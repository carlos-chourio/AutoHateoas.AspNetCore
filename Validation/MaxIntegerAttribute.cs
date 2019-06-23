using System.ComponentModel.DataAnnotations;

namespace AutoHateoas.AspNetCore.Validation {
    public class MaxIntegerAttribute : ValidationAttribute {
        public int MaximumNumber { get; set; }
        public override bool IsValid(object value) {
            if (int.TryParse(value.ToString(), out int x)) {
                return x <= MaximumNumber;
            }
            return false;
        }
    }
}
