using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CcLibrary.AspNetCore.Filters {
    internal static class FiltersHelper {
        internal static async Task<TParameter> GetParameterFromAction<TParameter>(ResultExecutingContext context, string parameterName) {
            var parameterDescriptor = context.ActionDescriptor.Parameters.Where(t => t.Name.Equals(parameterName)).FirstOrDefault();
            ControllerBase controller = context.Controller as ControllerBase;
            TParameter parameter = (TParameter)Activator.CreateInstance(parameterDescriptor.ParameterType);
            await controller.TryUpdateModelAsync(parameter, parameterDescriptor.ParameterType, string.Empty);
            return parameter;
        }
    }
}
