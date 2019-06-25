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
    /// Performs the pagination from an IQueryable object of type <typeparamref name="TDto"/>
    /// </summary>
    /// <typeparam name="TDto">The type data transfer object</typeparam>
    /// <typeparam name="TEntity">The type of the entity</typeparam>
    public class HateoasAutoPagination<TEntity, TDto> : IAsyncResultFilter  where TDto : IIdentityDto {
        private readonly IPaginationHelperService<TEntity> paginationHelperService;
        private readonly HateoasScanner filterConfiguration;
        private readonly LinkGenerator linkGenerator;

        public HateoasAutoPagination(IPaginationHelperService<TEntity> paginationHelperService, HateoasScanner filterConfiguration, LinkGenerator linkGenerator) {
            this.paginationHelperService = paginationHelperService ?? throw new ArgumentNullException(nameof(paginationHelperService));
            this.filterConfiguration = filterConfiguration ?? throw new ArgumentNullException(nameof(filterConfiguration));
            this.linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            var result = context.Result as ObjectResult;
            if (FiltersHelper.IsResponseSuccesful(result)) {
                PaginationModel<TEntity> paginationModel = 
                    await FiltersHelper.GetParameterFromActionAsync<PaginationModel<TEntity>>(context);
                IQueryable<TDto> list = result.Value as IQueryable<TDto>;
                var dtoPagedList = await list.ToPagedListAsync(paginationModel.PageSize, paginationModel.PageNumber);
                /// Doesn't support many pagination methods for a single controller
                var paginationMethodInfo  = filterConfiguration.ControllerInfoDictionary[context.Controller.GetType()].ControllerActions.Where(t=> t.ResourceType == Attributes.ResourceType.Collection).FirstOrDefault();
                string mediaType = FiltersHelper.GetValueFromHeader(context, "Accept");
                PaginationMetadata paginationMetadata = paginationHelperService.GeneratePaginationMetaData(dtoPagedList, paginationModel, context.Controller.GetType().Name, paginationMethodInfo.ActionName);
                if (filterConfiguration.SupportsCustomDataType && mediaType.Equals(filterConfiguration.CustomDataType, StringComparison.CurrentCultureIgnoreCase)) {
                    var controllerType = context.Controller.GetType();
                    var dtoPagedListWithExternalLinks = HateoasHelper.CreateLinksForCollectionResource(dtoPagedList, filterConfiguration, paginationMetadata, context.Controller.GetType());
                    var shapedDtoPagedListWithLinks = new EnvelopCollection<EnvelopDto<TDto>> {
                        Values = dtoPagedListWithExternalLinks.Values.Select(dto => {
                            return HateoasHelper
                                .CreateLinksForSingleResource(dto, filterConfiguration, linkGenerator, controllerType);
                        }), Links = dtoPagedListWithExternalLinks.Links
                    };
                    result.Value = shapedDtoPagedListWithLinks;
                } else {
                    result.Value = dtoPagedList;
                }
                await next();
            }
        }
    }
}
