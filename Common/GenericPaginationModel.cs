using AutoHateoas.AspNetCore.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoHateoas.AspNetCore.Common {
    public class PaginationModel<TEntity> : PaginationModel, IValidatableObject {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!PropertiesValidator.ValidatePropertiesExist(typeof(TEntity), FieldsRequested)) {
                yield return new ValidationResult("The fields requested are invalid", new [] { "FieldsRequested" });
            }
        }
    }
}