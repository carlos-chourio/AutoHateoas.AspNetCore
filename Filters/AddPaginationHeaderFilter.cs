using System.Threading.Tasks;
using CcLibrary.AspNetCore.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using CcLibrary.AspNetCore.Collections.Abstractions;
using CcLibrary.AspNetCore.Common;
using System.Linq;
using CcLibrary.AspNetCore.Extensions;

namespace CcLibrary.AspNetCore.Filters {

    /// <summary>
    /// Adds X-Pagination Header to the response.
    /// The value of the response should be of IQueryable of <typeparamref name="TEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">The entity</typeparam>
    public class AddPaginationHeaderFilter<TEntity> : IAsyncResultFilter {
        private readonly IPaginationHelperService<TEntity> paginationHelperService;
        private readonly FilterConfiguration filterConfiguration;

        public AddPaginationHeaderFilter(IPaginationHelperService<TEntity> paginationHelperService, FilterConfiguration filterConfiguration) {
            this.paginationHelperService = paginationHelperService ?? throw new System.ArgumentNullException(nameof(paginationHelperService));
            this.filterConfiguration = filterConfiguration ?? throw new System.ArgumentNullException(nameof(filterConfiguration));
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            var result = context.Result as ObjectResult;
            if (FiltersHelper.IsResponseSuccesful(result)) {
                PaginationModel<TEntity> paginationModel = await FiltersHelper.GetParameterFromAction<PaginationModel<TEntity>>(context);
                string controllerName = context.Controller.GetType().Name;
                IQueryable<TEntity> list = result.Value as IQueryable<TEntity>;
                var pagedList = await list.ToPagedListAsync(paginationModel.PageSize, paginationModel.PageNumber);
                var paginationMetadata = paginationHelperService.GeneratePaginationMetaData(pagedList, paginationModel, controllerName, "");
                FiltersHelper.AddPaginationHeaders(filterConfiguration, context, paginationMetadata);
                await next();
            }
        }

        
    }
}