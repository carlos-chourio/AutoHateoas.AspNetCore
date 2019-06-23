using AutoHateoas.AspNetCore.Attributes;
using AutoHateoas.AspNetCore.DTOs;
using AutoHateoas.AspNetCore.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoHateoas.AspNetCore.Common {
    internal static class HateoasHelper {
        internal static EnvelopDto<TDto> CreateLinksForSingleResource<TDto>(TDto dto, FilterConfiguration filterConfiguration, LinkGenerator linkGenerator, Type controllerType)
            where TDto : IIdentityDto {
            var envelop = new EnvelopDto<TDto>(dto);
            ControllerAction[] actionsFromController = filterConfiguration.ControllerInfoDictionary[controllerType]
                .ControllerActions.Where(t => t.ResourceType.Equals(ResourceType.Single)).ToArray();
            foreach (var action in actionsFromController) {
                if (action.MethodType.Equals("Get") || action.MethodType.Equals("Patch")) {
                    envelop.Links.Add(new LinkDto(linkGenerator.GetPathByAction(action.ActionName, controllerType.Name.Replace("Controller", ""), new { dto.Id }),
                    action.MethodName, action.MethodType));
                } else {
                    envelop.Links.Add(new LinkDto(linkGenerator.GetPathByAction(action.ActionName, controllerType.Name.Replace("Controller", "")),
                    action.MethodName, action.MethodType));
                }
            }
            return envelop;
        }

        internal static EnvelopCollection<TDto> CreateLinksForCollectionResource<TDto>(IEnumerable<TDto> dtoCollection, FilterConfiguration filterConfiguration, PaginationMetadata paginationMetadata, Type controllerType)
            where TDto : IIdentityDto {
            var action = filterConfiguration.ControllerInfoDictionary[controllerType].ControllerActions.First(t => t.ResourceType == ResourceType.Collection);
            var envelop = new EnvelopCollection<TDto>(dtoCollection);
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
    }
}
