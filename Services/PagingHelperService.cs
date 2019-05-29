using CcLibrary.AspNetCore.Collections.Abstractions;
using CcLibrary.AspNetCore.Common;
using CcLibrary.AspNetCore.Enumerations;
using CcLibrary.AspNetCore.Services.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace CcLibrary.AspNetCore.Services {
    public class PagingHelperService<TEntity> : IPagingHelperService<TEntity> {
        private readonly LinkGenerator linkGenerator;

        public PagingHelperService(LinkGenerator linkGenerator) {
            this.linkGenerator = linkGenerator;
        }
        public PagingMetadata GeneratePagingMetaData<T>(IPagedList<T> pagedData, PagingModel<TEntity> PagingModel, string controllerName, string methodName) {
            string previousPage = pagedData.HasPrevious ? CreateUri(PagingModel, ResourceUriType.Previous, controllerName, methodName) : null;
            string nextPage = pagedData.HasNext ? CreateUri(PagingModel, ResourceUriType.Next, controllerName, methodName) : null;
            string selfPage = CreateUri(PagingModel, ResourceUriType.Self, controllerName, methodName);
            return new PagingMetadata(pagedData.TotalCount, pagedData.PageSize, pagedData.CurrentPage, pagedData.TotalPages, previousPage, nextPage, selfPage);
        }
        private string CreateUri(PagingModel<TEntity> PagingModel, ResourceUriType uriType, string controllerName, string methodName) {
            int pageNumber = PagingModel.PageNumber;
            switch (uriType) {
                case ResourceUriType.Next:
                    pageNumber++;
                    break;
                case ResourceUriType.Previous:
                    pageNumber--;
                    break;
                case ResourceUriType.Self:
                    break;
                default:
                    break;
            }
            return linkGenerator.GetPathByAction(methodName, controllerName, new {
                fieldsRequested = PagingModel.FieldsRequested,
                searchQuery = PagingModel.SearchQuery,
                pageSize = PagingModel.PageSize,
                orderBy = PagingModel.OrderBy,
                ids = PagingModel.Ids,
                pageNumber
            });
        }
    }
}
