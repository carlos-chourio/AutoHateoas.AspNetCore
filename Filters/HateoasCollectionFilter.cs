using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CcLibrary.AspNetCore.Common;
using CcLibrary.AspNetCore.DTOs;
using CcLibrary.AspNetCore.Extensions;
using CcLibrary.AspNetCore.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace CcLibrary.AspNetCore.Filters {
    /// <summary>
    /// Adds Hateoas Links for Paginated Resources of type <typeparamref name="TEntity"/>.
    /// Converts the object result to a <typeparamref name="TDto"/> collection with Hateoas Links
    /// The Object Result must be a tuple of type (resouce:object, paginationMetadata:PaginationMetadata)
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity</typeparam>
    /// <typeparam name="TDto">The type data transfer object</typeparam>
    public class HateoasForCollection<TEntity, TDto> : IAsyncResultFilter  where TDto : IIdentityDto {
        private readonly IPaginationHelperService<TEntity> paginationHelperService;
        private readonly FilterConfiguration filterConfiguration;
        private readonly IMapper mapper;
        private readonly LinkGenerator linkGenerator;

        public HateoasForCollection(IPaginationHelperService<TEntity> paginationHelperService, FilterConfiguration filterConfiguration, IMapper mapper, LinkGenerator linkGenerator) {
            this.paginationHelperService = paginationHelperService ?? throw new ArgumentNullException(nameof(paginationHelperService));
            this.filterConfiguration = filterConfiguration ?? throw new ArgumentNullException(nameof(filterConfiguration));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            var result = context.Result as ObjectResult;
            if (FiltersHelper.IsResponseSuccesful(result)) {
                PaginationModel<TEntity> paginationModel = 
                    await FiltersHelper.GetParameterFromActionAsync<PaginationModel<TEntity>>(context);
                (IEnumerable<TEntity> values,PaginationInfo metadata) resultValue = ((IEnumerable<TEntity>,PaginationMetadata)) result.Value;
                var paginationMethodInfo  = filterConfiguration.ControllerInfoDictionary[context.Controller.GetType()].ControllerActions.Where(t=> t.ResourceType == Attributes.ResourceType.Collection).FirstOrDefault();
                var paginationMetadata = paginationHelperService.GeneratePaginationMetaData(resultValue.metadata,paginationModel, context.Controller.GetType().Name, paginationMethodInfo.ActionName);
                string mediaType = FiltersHelper.GetValueFromHeader(context, "Accept");
                var dtoPagedList = mapper.Map<IEnumerable<TDto>>(resultValue.values); 
                if (filterConfiguration.SupportsCustomDataType && mediaType.Equals(filterConfiguration.CustomDataType, StringComparison.CurrentCultureIgnoreCase)) {
                    var controllerType = context.Controller.GetType();
                    var dtoPagedListWithExternalLinks = FiltersHelper.CreateLinksForCollectionResource(dtoPagedList, filterConfiguration, paginationMetadata, context.Controller.GetType());
                    var shapedDtoPagedListWithLinks = new EnvelopCollectionDto<ExpandoObject> {
                        Items = dtoPagedListWithExternalLinks.Items.Select(dto => {
                            return FiltersHelper
                                .CreateLinksForSingleResource(dto, filterConfiguration, linkGenerator, controllerType)
                                .ShapeDataWithRequestedFields(paginationModel.FieldsRequested, true);
                        }), Links = dtoPagedListWithExternalLinks.Links
                    };
                    result.Value = shapedDtoPagedListWithLinks;
                } else {
                    result.Value = dtoPagedList.ShapeCollectionDataWithRequestedFields(paginationModel.FieldsRequested, true);
                }
                await next();
            }
        }
    }
}
