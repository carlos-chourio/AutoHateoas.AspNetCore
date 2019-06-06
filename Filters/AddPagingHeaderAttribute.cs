using System;
using System.Threading.Tasks;
using CcLibrary.AspNetCore.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Linq;
using CcLibrary.AspNetCore.Collections.Abstractions;
using CcLibrary.AspNetCore.Common;

namespace CcLibrary.AspNetCore.Filters {
    /// <summary>
    /// Adds X-pagination Header to the response.
    /// This attribute requires the Result to be a tuple
    /// where the 1st element is the value and the 2nd element is the PagingModel
    /// </summary>
    public class AddPaginationHeaderFilter<TEntity> : IAsyncResultFilter {
        private readonly IPagingHelperService<TEntity> pagingHelperService;
        #region old code
        //public override void OnResultExecuting(ResultExecutingContext context) {
        //    var result = context.Result as ObjectResult;
        //    if (result?.Value != null && result?.StatusCode >= 200 &&
        //        result?.StatusCode < 300) {
        //        (dynamic actualValue, PagingMetadata pagingMetadata) = ((dynamic, PagingMetadata))result.Value;
        //        string paging = (context.HttpContext.Request.Headers["Accept"] == "application/vnd.quiniela.hateoas+json")
        //            ? JsonConvert.SerializeObject(pagingMetadata)
        //            : JsonConvert.SerializeObject(pagingMetadata.ToMiniPagingMetadata());
        //        context.HttpContext.Response.Headers.Add("X-Pagination", paging);
        //        result.Value = actualValue;
        //    }
        //} 
        #endregion
        public AddPaginationHeaderFilter(IPagingHelperService<TEntity> pagingHelperService) {
            this.pagingHelperService = pagingHelperService;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next) {
            var result = context.Result as ObjectResult;
            if (result?.Value != null && result?.StatusCode >= 200 &&
                result?.StatusCode < 300) {
                var parameterDescriptor = context.ActionDescriptor.Parameters.Where(t=>t.Name.Equals("pagingViewModel")).FirstOrDefault();
                ControllerBase controller = context.Controller as ControllerBase;
                PagingModel<TEntity> pagingModel = (PagingModel<TEntity>) Activator.CreateInstance(parameterDescriptor.ParameterType);
                await controller.TryUpdateModelAsync(pagingModel, parameterDescriptor.ParameterType, string.Empty);
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


