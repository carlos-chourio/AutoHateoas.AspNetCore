using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AutoHateoas.AspNetCore.Common;
using AutoHateoas.AspNetCore.DTOs;
using AutoHateoas.AspNetCore.Extensions;
using AutoHateoas.AspNetCore.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace AutoHateoas.AspNetCore.Filters {
    /// <summary>
    /// Adds Hateoas Links for Paginated Resources of type <typeparamref name="TDto"/>.
    /// The object returned by the action must be of type PaginationResult
    /// </summary>
    /// <typeparam name="TDto">The type the data transfer object</typeparam>
    public class HateoasForCollection<TEntity, TDto> : IAsyncResultFilter  where TDto : IIdentityDto {
        private readonly IPaginationHelperService paginationHelperService;
        private readonly FilterConfiguration filterConfiguration;
        private readonly LinkGenerator linkGenerator;

        public HateoasForCollection(IPaginationHelperService paginationHelperService, FilterConfiguration filterConfiguration, LinkGenerator linkGenerator) {
            this.paginationHelperService = paginationHelperService ?? throw new ArgumentNullException(nameof(paginationHelperService));
            this.filterConfiguration = filterConfiguration ?? throw new ArgumentNullException(nameof(filterConfiguration));
            this.linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            var result = context.Result as ObjectResult;
            if (FiltersHelper.IsResponseSuccesful(result)) {
                PaginationModel<TEntity> paginationModel = await FiltersHelper.GetParameterFromActionAsync<PaginationModel<TEntity>>(context);
                PaginatedResult resultValue = (PaginatedResult) result.Value;
                // Currently only supports one Collection action in a controller
                var paginationMethodInfo  = filterConfiguration.ControllerInfoDictionary[context.Controller.GetType()].ControllerActions.Where(t=> t.ResourceType == Attributes.ResourceType.Collection).FirstOrDefault();
                PaginationMetadata paginationMetadata = paginationHelperService.GeneratePaginationMetaData(resultValue.PaginationInfo, paginationModel, context.Controller.GetType().Name, paginationMethodInfo.ActionName);
                string mediaType = FiltersHelper.GetValueFromHeader(context, "Accept");
                IEnumerable<TDto> pagedList = (IEnumerable<TDto>)resultValue.Value;
                if (filterConfiguration.SupportsCustomDataType && mediaType.Equals(filterConfiguration.CustomDataType, StringComparison.CurrentCultureIgnoreCase)) {
                    EnvelopCollection<ExpandoObject> shapedPagedListWithLinks = AddLinksAndShapeData(context, paginationModel, paginationMetadata, pagedList);
                    result.Value = shapedPagedListWithLinks;
                } else {
                    result.Value = pagedList.ShapeCollectionDataWithRequestedFields(paginationModel.FieldsRequested, true);
                }
                await next();
            }
        }

        private EnvelopCollection<ExpandoObject> AddLinksAndShapeData(ResultExecutingContext context, PaginationModel<TEntity> paginationModel, PaginationMetadata paginationMetadata, IEnumerable<TDto> pagedList) {
            Type controllerType = context.Controller.GetType();
            EnvelopCollection<TDto> pagedListWithExternalLinks = HateoasHelper.CreateLinksForCollectionResource(pagedList, filterConfiguration, paginationMetadata, context.Controller.GetType());
            EnvelopCollection<ExpandoObject> shapedPagedListWithLinks = new EnvelopCollection<ExpandoObject>
            {
                Items = pagedListWithExternalLinks.Items.Select(dto => {
                    return HateoasHelper
                        .CreateLinksForSingleResource(dto, filterConfiguration, linkGenerator, controllerType)
                        .ShapeDataWithRequestedFields(paginationModel.FieldsRequested, true);
                }), Links = pagedListWithExternalLinks.Links
            };
            return shapedPagedListWithLinks;
        }
    }
}
