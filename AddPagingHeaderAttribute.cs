using System;
using System.Threading.Tasks;
using CcLibrary.AspNetCore.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace CcLibrary.AspNetCore {
    /// <summary>
    /// Adds X-pagination Header to the response.
    /// This attribute requires the Result to be a tuple
    /// where the 1st element is the value and the 2nd element is the PagingModel
    /// </summary>
    public class AddPagingHeaderAttribute : ResultFilterAttribute {
        public override void OnResultExecuting(ResultExecutingContext context) {
            var result = context.Result as ObjectResult;
            if (result?.Value != null && result?.StatusCode >= 200 &&
                result?.StatusCode < 300) {
                (dynamic actualValue, PagingMetadata pagingMetadata) = ((dynamic, PagingMetadata))result.Value;
                string paging = (context.HttpContext.Request.Headers["Accept"] == "application/vnd.quiniela.hateoas+json")
                    ? JsonConvert.SerializeObject(pagingMetadata)
                    : JsonConvert.SerializeObject(pagingMetadata.ToMiniPagingMetadata());
                context.HttpContext.Response.Headers.Add("X-Pagination", paging);
                result.Value = actualValue;
            }
        }
    }
}


