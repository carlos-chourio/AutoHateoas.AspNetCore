using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CcLibrary.AspNetCore.Filters {
    internal static class FiltersHelper {
        internal static async Task<TParameter> GetParameterFromAction<TParameter>(ResultExecutingContext context) {
            var parameterDescriptor = context.ActionDescriptor.Parameters.Where(t => t.ParameterType.Equals(typeof(TParameter))).FirstOrDefault();
            return await GetParameterFromParameterDescriptor<TParameter>(context, parameterDescriptor);
        }

        internal static async Task<TParameter> GetParameterFromAction<TParameter>(ResultExecutingContext context, string parameterName) {
            var parameterDescriptor = context.ActionDescriptor.Parameters.Where(t => t.Name.Equals(parameterName)).FirstOrDefault();
            return await GetParameterFromParameterDescriptor<TParameter>(context, parameterDescriptor);
        }

        internal static string GetValueFromQueryString(ResultExecutingContext context, string key) {
            if (context.HttpContext.Request.Query.TryGetValue(key, out StringValues fields)) {
                return fields.ToString();
            }
            return string.Empty;
        }

        internal static string GetValueFromHeader(ResultExecutingContext context, string key) {
            return context.HttpContext.Request.Headers[key].ToString();
        }

        internal static bool IsResponseSuccesful(ObjectResult result) {
            return result?.Value != null && result?.StatusCode >= 200 && result?.StatusCode < 300;
        }

        private static async Task<TParameter> GetParameterFromParameterDescriptor<TParameter>(ResultExecutingContext context, ParameterDescriptor parameterDescriptor) {
            ControllerBase controller = context.Controller as ControllerBase;
            TParameter parameter = (TParameter)Activator.CreateInstance(parameterDescriptor.ParameterType);
            await controller.TryUpdateModelAsync(parameter, parameterDescriptor.ParameterType, string.Empty);
            return parameter;
        }
    }
}
