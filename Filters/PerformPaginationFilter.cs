using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CcLibrary.AspNetCore.Common;
using CcLibrary.AspNetCore.DTOs;
using CcLibrary.AspNetCore.Extensions;
using CcLibrary.AspNetCore.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CcLibrary.AspNetCore.Filters {
    /// <summary>
    /// Performs the pagination from an IQueryable object of type <typeparamref name="TEntity"/>
    /// and converts the object result to a Paged List of type <typeparamref name="TDto"/>
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity</typeparam>
    /// <typeparam name="TDto">The type data transfer object</typeparam>
    public class PerformPaginationFilter<TEntity, TDto> : IAsyncResultFilter  where TDto : IIdentityDto {
        private readonly IPaginationHelperService<TEntity> paginationHelperService;
        private readonly FilterConfiguration filterConfiguration;
        private readonly IMapper mapper;

        public PerformPaginationFilter(IPaginationHelperService<TEntity> paginationHelperService, FilterConfiguration filterConfiguration, IMapper mapper) {
            this.paginationHelperService = paginationHelperService ?? throw new ArgumentNullException(nameof(paginationHelperService));
            this.filterConfiguration = filterConfiguration ?? throw new ArgumentNullException(nameof(filterConfiguration));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            var result = context.Result as ObjectResult;
            if (FiltersHelper.IsResponseSuccesful(result)) {
                PaginationModel<TEntity> paginationModel = await FiltersHelper.GetParameterFromAction<PaginationModel<TEntity>>(context);
                IQueryable<TEntity> list = result.Value as IQueryable<TEntity>;
                var pagedList = await list.ToPagedListAsync(paginationModel.PageSize, paginationModel.PageNumber);
                /// Doesn't support many pagination methods for a single controller
                var paginationMethodInfo  = filterConfiguration.ControllerInfoDictionary[context.Controller.GetType()].ControllerActions.Where(t=> t.ResourceType == Attributes.ResourceType.Collection).FirstOrDefault();
                string mediaType = FiltersHelper.GetValueFromHeader(context, "Accept");
                PaginationMetadata paginationMetadata = paginationHelperService.GeneratepaginationMetaData(pagedList, paginationModel, context.Controller.GetType().Name, paginationMethodInfo.MethodName);
                var dtoPagedList = mapper.Map<IEnumerable<TDto>>(pagedList);
                if (mediaType.Equals(filterConfiguration.CustomDataType, StringComparison.CurrentCultureIgnoreCase)) {
                    //Missing link generation
                    #region old code
                    //var pageWithLinks = quinielasDto.Select(quiniela => {
                    //    return QuinielaHelper.AddLinksForQuiniela(quiniela, quiniela.Id, linkGenerator)
                    //    .ShapeDataWithRequestedFields(paginationViewModel.FieldsRequested, true);
                    //});
                    //return Ok(QuinielaHelper.AddLinksForQuinielapaginationCollection(quinielaDtoPageWithLinks, paginationModel, paginationMetadata)); 
                    #endregion
                } else {
                    result.Value = dtoPagedList.ShapeCollectionDataWithRequestedFields(paginationModel.FieldsRequested, true);
                }

                await next();
            }
        }
    }
}
