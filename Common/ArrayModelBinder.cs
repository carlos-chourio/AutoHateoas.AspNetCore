using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHateoas.AspNetCore.Common {
    public class ArrayModelBinder<TType> : IModelBinder {
        public Task BindModelAsync(ModelBindingContext bindingContext) {
            if (bindingContext.ModelMetadata.IsEnumerableType) {
                var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();
                if (!string.IsNullOrEmpty(value)) {
                    var elementType = typeof(TType); 
                    var typeConverter = TypeDescriptor.GetConverter(elementType);
                    var splittedValues = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var values = splittedValues.Select(t => typeConverter.ConvertFromString(t.Trim())).ToArray();
                    var typedValues = Array.CreateInstance(elementType, values.Length);
                    values.CopyTo(typedValues, 0);
                    bindingContext.Model = typedValues;
                    return SuccessBinding(bindingContext, typedValues);
                }
                return SuccessBinding(bindingContext, null);
            }
            return FailedBinding(bindingContext);
        }

        private static Task SuccessBinding(ModelBindingContext bindingContext, Array typedValues) {
            bindingContext.Result = ModelBindingResult.Success(typedValues);
            return Task.CompletedTask;
        }

        private static Task FailedBinding(ModelBindingContext bindingContext) {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }
    }
}
