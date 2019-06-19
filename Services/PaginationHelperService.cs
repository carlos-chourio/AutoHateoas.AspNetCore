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

        public PaginationMetadata GeneratePaginationMetaData<T>(IPagedList<T> pagedData, PaginationModel<TEntity> paginationModel, string controllerName, string actionName)
        {
            string previousPage, nextPage, selfPage;
            GeneratePaginationLinks(pagedData.HasPrevious, pagedData.HasNext, paginationModel, controllerName, actionName, out previousPage, out nextPage, out selfPage);
            return new PaginationMetadata(pagedData.TotalCount, pagedData.PageSize, pagedData.CurrentPage, pagedData.TotalPages, previousPage, nextPage, selfPage);
        }

        public PaginationMetadata GeneratePaginationMetaData(PaginationInfo paginationInfo, PaginationModel<TEntity> paginationModel, string controllerName, string actionName)
        {
            string previousPage, nextPage, selfPage;
            GeneratePaginationLinks(paginationInfo.CurrentPage>1, paginationInfo.CurrentPage<paginationInfo.TotalPages, paginationModel, controllerName, actionName, out previousPage, out nextPage, out selfPage);
            return new PaginationMetadata(paginationInfo.TotalCount, paginationInfo.PageSize, paginationInfo.CurrentPage, paginationInfo.TotalPages, previousPage, nextPage, selfPage);
        }

        private void GeneratePaginationLinks(bool hasPrevious, bool hasNext, PaginationModel<TEntity> paginationModel, string controllerName, string actionName, out string previousPage, out string nextPage, out string selfPage)
        {
            previousPage = hasPrevious ? CreateUri(paginationModel, ResourceUriType.Previous, controllerName, actionName) : null;
            nextPage = hasNext ? CreateUri(paginationModel, ResourceUriType.Next, controllerName, actionName) : null;
            selfPage = CreateUri(paginationModel, ResourceUriType.Self, controllerName, actionName);
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
