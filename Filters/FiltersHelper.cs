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
                                : JsonConvert.SerializeObject(paginationMetadata.ToMinipaginationMetadata());
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
