using System.Threading.Tasks;
using CcLibrary.AspNetCore.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using CcLibrary.AspNetCore.Collections.Abstractions;
using CcLibrary.AspNetCore.Common;

namespace CcLibrary.AspNetCore.Filters {
    /// <summary>
    /// Adds X-Pagination Header to the response.
    /// The response type should be an IPagedList of <typeparamref name="TEntity"/>
    /// </summary>
    public class AddPaginationHeaderFilter<TEntity> : IAsyncResultFilter {
        private readonly IPaginationHelperService<TEntity> pagingHelperService;

        public AddPaginationHeaderFilter(IPaginationHelperService<TEntity> paginationHelperService) {
            this.pagingHelperService = paginationHelperService;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            var result = context.Result as ObjectResult;
            if (result?.Value != null && result?.StatusCode >= 200 &&
                result?.StatusCode < 300) {
                PagingModel<TEntity> pagingModel = await FiltersHelper.GetParameterFromAction<PagingModel<TEntity>>(context, "pagingViewModel");
                string controllerName = context.Controller.GetType().Name;
                IPagedList<TEntity> pagedList = result.Value as IPagedList<TEntity>;
                var pagingMetadata = pagingHelperService.GeneratePagingMetaData(pagedList, pagingModel, controllerName, "");
                string paging = (context.HttpContext.Request.Headers["Accept"] == "application/vnd.quiniela.hateoas+json")
                    ? JsonConvert.SerializeObject(pagingMetadata)
                    : JsonConvert.SerializeObject(pagingMetadata.ToMiniPagingMetadata());
                context.HttpContext.Response.Headers.Add("X-Pagination", paging);
                await next();
            }
        }
    }
}


