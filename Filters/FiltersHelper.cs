using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AutoHateoas.AspNetCore.Filters {
    internal static class FiltersHelper {
        /// <summary>
        /// Gets a parameter of type <typeparamref name="TParameter"/> from the Action by inspectioning the type.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter you want to get from the the Action</typeparam>
        /// <param name="context">The Context of the Result Filter</param>
        /// <returns>The Parameter of type <typeparamref name="TParameter"/></returns>
        internal static async Task<TParameter> GetParameterFromActionAsync<TParameter>(ResultExecutingContext context) {
            var parameterDescriptor = context.ActionDescriptor.Parameters.Where(t => t.ParameterType.Equals(typeof(TParameter))).FirstOrDefault();
            return await GetParameterFromParameterDescriptor<TParameter>(context, parameterDescriptor);
        }
        
        /// <summary>
        /// Gets a parameter of type <typeparamref name="TParameterBaseType"/> from the Action by inspectioning the base type of the actual parameter.
        /// </summary>
        /// <typeparam name="TParameterBaseType">The type of the base-parameter to return from the the Action </typeparam>
        /// <param name="context">The Context of the Result Filter</param>
        /// <returns>The actual Parameter casted to the type <typeparamref name="TParameterBaseType"/></returns>
        internal static async Task<TParameterBaseType> GetParameterKnowingBaseTypeFromActionAsync<TParameterBaseType>(ResultExecutingContext context) {
            var parameterDescriptor = context.ActionDescriptor.Parameters.Where(t => t.ParameterType.BaseType.Equals(typeof(TParameterBaseType))).FirstOrDefault();
            return await GetParameterFromParameterDescriptor<TParameterBaseType>(context, parameterDescriptor);
        }

        /// <summary>
        /// Gets a parameter of type <typeparamref name="TParameter"/> from the Action by inspectioning <paramref name="parentType"/>.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter you want to get from the the Action</typeparam>
        /// <param name="context">The Context of the Result Filter</param>
        /// <param name="childType">The Child type of the parameter</param>
        /// <returns>The Parameter of type <typeparamref name="TParameter"/></returns>
        internal static async Task<TParameter> GetParameterFromActionAsync<TParameter>(ResultExecutingContext context, Type childType) {
            var parameterDescriptor = context.ActionDescriptor.Parameters.Where(t => t.ParameterType.Equals(childType)).FirstOrDefault();
            return await GetParameterFromParameterDescriptor<TParameter>(context, parameterDescriptor);
        }

        /// <summary>
        /// Gets a parameter of type <typeparamref name="TParameter"/> from the Action by inspectioning the <paramref name="parameterName"/>.
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter you want to get from the the Action</typeparam>
        /// <param name="context">The Context of the Result Filter</param>
        /// <param name="parameterName">The name of the parameter to find</param>
        /// <returns>The Parameter of type <typeparamref name="TParameter"/></returns>
        internal static async Task<TParameter> GetParameterFromActionAsync<TParameter>(ResultExecutingContext context, string parameterName) {
            var parameterDescriptor = context.ActionDescriptor.Parameters.Where(t => t.Name.Equals(parameterName)).FirstOrDefault();
            return await GetParameterFromParameterDescriptor<TParameter>(context, parameterDescriptor);
        }

        /// <summary>
        /// Gets the value of a <paramref name="key"/> from the query string inside the <paramref name="context"/>
        /// </summary>
        /// <param name="context">The Context of the Result Filter</param>
        /// <param name="key">The Key to be searched inside the Query String</param>
        /// <returns>The value from the query string</returns>
        internal static string GetValueFromQueryString(ResultExecutingContext context, string key) {
            if (context.HttpContext.Request.Query.TryGetValue(key, out StringValues fields)) {
                return fields.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the value of a <paramref name="key"/> from the Headers inside the <paramref name="context"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <returns>The value from the header</returns>
        internal static string GetValueFromHeader(ResultExecutingContext context, string key) {
            return context.HttpContext.Request.Headers[key].ToString();
        }

        /// <summary>
        /// Checks if the response was Succesful
        /// </summary>
        /// <param name="result">The result from the action.</param>
        /// <returns>true if succesful, otherwise false</returns>
        internal static bool IsResponseSuccesful(ObjectResult result) {
            return result?.Value != null && result?.StatusCode >= 200 && result?.StatusCode < 300;
        }

        internal static string GetControllerName(ResultExecutingContext context) {
            return context.Controller.GetType().Name.Replace("Controller", "");
        }

        private static async Task<TParameter> GetParameterFromParameterDescriptor<TParameter>(ResultExecutingContext context, ParameterDescriptor parameterDescriptor) {
            ControllerBase controller = context.Controller as ControllerBase;
            TParameter parameter = (TParameter)Activator.CreateInstance(parameterDescriptor.ParameterType);
            await controller.TryUpdateModelAsync(parameter, parameterDescriptor.ParameterType, string.Empty);
            return parameter;
        }

        private static async Task<object> GetParameterFromParameterDescriptor(ResultExecutingContext context, ParameterDescriptor parameterDescriptor) {
            ControllerBase controller = context.Controller as ControllerBase;
            object parameter = Activator.CreateInstance(parameterDescriptor.ParameterType);
            await controller.TryUpdateModelAsync(parameter, parameterDescriptor.ParameterType, string.Empty);
            return parameter;
        }
    }
}
