using CcLibrary.AspNetCore.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CcLibrary.AspNetCore.Common {
    public class PagingModel<TEntity> : PagingModel, IValidatableObject {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!PropertiesValidator.ValidatePropertiesExist(typeof(TEntity), FieldsRequested)) {
                yield return new ValidationResult("The fields requested are invalid", new [] { "FieldsRequested" });
            }
        }
    }
}