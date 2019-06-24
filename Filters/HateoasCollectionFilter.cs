using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoHateoas.AspNetCore.Common;
using AutoHateoas.AspNetCore.DTOs;
using AutoHateoas.AspNetCore.Extensions;
using AutoHateoas.AspNetCore.Services.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace AutoHateoas.AspNetCore.Filters {
    /// <summary>
    /// Adds Hateoas Links for Paginated Resources of type <typeparamref name="TDto"/>.
    /// The object returned by the action must be of type PaginationResult
    /// </summary>
    /// <typeparam name="TDto">The type the data transfer object</typeparam>
    public class HateoasForCollection<TEntity, TDto> : IAsyncResultFilter where TDto : IIdentityDto {
        private readonly IPaginationHelperService<TEntity> paginationHelperService;
        private readonly FilterConfiguration filterConfiguration;
        private readonly LinkGenerator linkGenerator;

        public HateoasForCollection(IPaginationHelperService<TEntity> paginationHelperService, FilterConfiguration filterConfiguration, LinkGenerator linkGenerator) {
            this.paginationHelperService = paginationHelperService ?? throw new ArgumentNullException(nameof(paginationHelperService));
            this.filterConfiguration = filterConfiguration ?? throw new ArgumentNullException(nameof(filterConfiguration));
            this.linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            PaginatedResult result = context.Result as PaginatedResult;
            if (FiltersHelper.IsResponseSuccesful(result)) {
                PaginationModel paginationModel = await FiltersHelper.GetParameterKnowingBaseTypeFromActionAsync<PaginationModel>(context);
                // Currently only supports one Collection action in a controller
                var paginationMethodInfo = filterConfiguration.ControllerInfoDictionary[context.Controller.GetType()].ControllerActions.Where(t => t.ResourceType == Attributes.ResourceType.Collection).FirstOrDefault();
                PaginationMetadata paginationMetadata = paginationHelperService.GeneratePaginationMetaData(result.PaginationInfo, paginationModel, FiltersHelper.GetControllerName(context), paginationMethodInfo.ActionName);
                string mediaType = FiltersHelper.GetValueFromHeader(context, "Accept");
                IEnumerable<TDto> pagedList = (IEnumerable<TDto>)result.Value;
                if (filterConfiguration.SupportsCustomDataType && mediaType.Equals(filterConfiguration.CustomDataType, StringComparison.CurrentCultureIgnoreCase)) {
                    EnvelopCollection<EnvelopDto<TDto>> pagedListWithLinks = AddInternalAndExternalLinks(context, paginationMetadata, pagedList);
                    result.Value = pagedListWithLinks;
                } else {
                    result.Value = pagedList;
                }
                await next();
            }
        }

        private EnvelopCollection<EnvelopDto<TDto>> AddInternalAndExternalLinks(ResultExecutingContext context, PaginationMetadata paginationMetadata, IEnumerable<TDto> pagedList) {
            Type controllerType = context.Controller.GetType();
            EnvelopCollection<TDto> pagedListWithExternalLinks = HateoasHelper.CreateLinksForCollectionResource(pagedList, filterConfiguration, paginationMetadata, context.Controller.GetType());
            EnvelopCollection<EnvelopDto<TDto>> shapedPagedListWithLinks = new EnvelopCollection<EnvelopDto<TDto>>
            {
                Values = pagedListWithExternalLinks.Values.Select(dto => {
                    return HateoasHelper
                        .CreateLinksForSingleResource(dto, filterConfiguration, linkGenerator, controllerType);
                }),
                Links = pagedListWithExternalLinks.Links
            };
            return shapedPagedListWithLinks;
        }
    }
}
