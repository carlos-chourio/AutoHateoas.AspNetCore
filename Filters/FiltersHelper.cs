using CcLibrary.AspNetCore.Attributes;
using CcLibrary.AspNetCore.Common;
using CcLibrary.AspNetCore.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CcLibrary.AspNetCore.Filters {
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

        internal static EnvelopDto<TDto> CreateLinksForSingleResource<TDto>(TDto dto, FilterConfiguration filterConfiguration, LinkGenerator linkGenerator, Type controllerType) 
            where TDto : IIdentityDto {
            var envelop = new EnvelopDto<TDto>(dto);
            ControllerAction[] actionsFromController = filterConfiguration.ControllerInfoDictionary[controllerType]
                .ControllerActions.Where(t => t.ResourceType.Equals(ResourceType.Single)).ToArray();
            foreach (var action in actionsFromController) {
                if (action.MethodType.Equals("Get") || action.MethodType.Equals("Patch")) {
                    envelop.Links.Add(new LinkDto(linkGenerator.GetPathByAction(action.ActionName, controllerType.Name.Replace("Controller",""), new { dto.Id }),
                    action.MethodName, action.MethodType));
                } else {
                    envelop.Links.Add(new LinkDto(linkGenerator.GetPathByAction(action.ActionName, controllerType.Name.Replace("Controller", "")),
                    action.MethodName, action.MethodType));
                }
            }
            return envelop;
        }

        internal static EnvelopCollectionDto<TDto> CreateLinksForCollectionResource<TDto>(IEnumerable<TDto> dtoCollection, FilterConfiguration filterConfiguration, PaginationMetadata paginationMetadata, Type controllerType) 
            where TDto : IIdentityDto {
            var action = filterConfiguration.ControllerInfoDictionary[controllerType].ControllerActions.First(t=> t.ResourceType== ResourceType.Collection);
            var envelop = new EnvelopCollectionDto<TDto>(dtoCollection);
            envelop.Links.Add(new LinkDto(paginationMetadata.NextPageLink, $"{action.MethodName}-next", action.MethodType));
            envelop.Links.Add(new LinkDto(paginationMetadata.PreviousPageLink, $"{action.MethodName}-previous", action.MethodType));
            envelop.Links.Add(new LinkDto(paginationMetadata.SelfPageLink, $"{action.MethodName}", action.MethodType));
            return envelop;
        }

        internal static void AddPaginationHeaders(FilterConfiguration filterConfiguration, ResultExecutingContext context, PaginationMetadata paginationMetadata) {
            string pagination = (filterConfiguration.SupportsCustomDataType &&
                                context.HttpContext.Request.Headers["Accept"].Equals(filterConfiguration.CustomDataType))
                                ? JsonConvert.SerializeObject(paginationMetadata)
                                : JsonConvert.SerializeObject(paginationMetadata.ToPaginationInfo());
            context.HttpContext.Response.Headers.Add("X-Pagination", pagination);
        }

        private static async Task<TParameter> GetParameterFromParameterDescriptor<TParameter>(ResultExecutingContext context, ParameterDescriptor parameterDescriptor) {
            ControllerBase controller = context.Controller as ControllerBase;
            TParameter parameter = (TParameter)Activator.CreateInstance(parameterDescriptor.ParameterType);
            await controller.TryUpdateModelAsync(parameter, parameterDescriptor.ParameterType, string.Empty);
            return parameter;
        }
    }
}
