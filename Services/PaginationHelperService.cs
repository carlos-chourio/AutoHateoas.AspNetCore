using CcLibrary.AspNetCore.Collections.Abstractions;
using CcLibrary.AspNetCore.Common;
using CcLibrary.AspNetCore.Enumerations;
using CcLibrary.AspNetCore.Services.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace CcLibrary.AspNetCore.Services {
    internal class PaginationHelperService<TEntity> : IPaginationHelperService<TEntity> {
        private readonly LinkGenerator linkGenerator;

        public PaginationHelperService(LinkGenerator linkGenerator) {
            this.linkGenerator = linkGenerator;
        }

        public PaginationMetadata GeneratePaginationMetaData<T>(IPagedList<T> pagedData, PaginationModel<TEntity> paginationModel, string controllerName, string actionName) {
            string previousPage = pagedData.HasPrevious ? CreateUri(paginationModel, ResourceUriType.Previous, controllerName, actionName) : null;
            string nextPage = pagedData.HasNext ? CreateUri(paginationModel, ResourceUriType.Next, controllerName, actionName) : null;
            string selfPage = CreateUri(paginationModel, ResourceUriType.Self, controllerName, actionName);
            return new PaginationMetadata(pagedData.TotalCount, pagedData.PageSize, pagedData.CurrentPage, pagedData.TotalPages, previousPage, nextPage, selfPage);
        }

        private string CreateUri(PaginationModel<TEntity> PaginationModel, ResourceUriType uriType, string controllerName, string methodName) {
            int pageNumber = PaginationModel.PageNumber;
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
                fieldsRequested = PaginationModel.FieldsRequested,
                searchQuery = PaginationModel.SearchQuery,
                pageSize = PaginationModel.PageSize,
                orderBy = PaginationModel.OrderBy,
                ids = PaginationModel.Ids,
                pageNumber
            });
        }
    }
}
