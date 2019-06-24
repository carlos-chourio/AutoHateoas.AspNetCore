using AutoHateoas.AspNetCore.Collections.Abstractions;
using AutoHateoas.AspNetCore.Common;
using AutoHateoas.AspNetCore.Enumerations;
using AutoHateoas.AspNetCore.Filters;
using AutoHateoas.AspNetCore.Services.Abstractions;
using Microsoft.AspNetCore.Routing;
using System;

namespace AutoHateoas.AspNetCore.Services {
    internal class PaginationHelperService<TEntity> : IPaginationHelperService<TEntity> {
        private readonly LinkGenerator linkGenerator;
        private readonly FilterConfiguration filterConfiguration;

        public PaginationHelperService(LinkGenerator linkGenerator, FilterConfiguration filterConfiguration) {
            this.linkGenerator = linkGenerator;
            this.filterConfiguration = filterConfiguration;
        }

        public PaginationMetadata GeneratePaginationMetaData<T>(IPagedList<T> pagedData, PaginationModel paginationModel, string controllerName, string actionName) {
            string previousPage, nextPage, selfPage;
            GeneratePaginationLinks(pagedData.HasPrevious, pagedData.HasNext, paginationModel, controllerName, actionName, out previousPage, out nextPage, out selfPage);
            return new PaginationMetadata(pagedData.TotalCount, pagedData.PageSize, pagedData.CurrentPage, pagedData.TotalPages, previousPage, nextPage, selfPage);
        }

        public PaginationMetadata GeneratePaginationMetaData(PaginationInfo paginationInfo, PaginationModel paginationModel, string controllerName, string actionName) {
            string previousPage, nextPage, selfPage;
            GeneratePaginationLinks(paginationInfo.CurrentPage > 1, paginationInfo.CurrentPage < paginationInfo.TotalPages, paginationModel, controllerName, actionName, out previousPage, out nextPage, out selfPage);
            return new PaginationMetadata(paginationInfo.TotalCount, paginationInfo.PageSize, paginationInfo.CurrentPage, paginationInfo.TotalPages, previousPage, nextPage, selfPage);
        }

        private void GeneratePaginationLinks(bool hasPrevious, bool hasNext, PaginationModel paginationModel, string controllerName, string actionName, out string previousPage, out string nextPage, out string selfPage) {
            previousPage = hasPrevious ? CreateUri(paginationModel, ResourceUriType.Previous, controllerName, actionName) : null;
            nextPage = hasNext ? CreateUri(paginationModel, ResourceUriType.Next, controllerName, actionName) : null;
            selfPage = CreateUri(paginationModel, ResourceUriType.Self, controllerName, actionName);
        }

        private string CreateUri(PaginationModel paginationModel, ResourceUriType uriType, string controllerName, string methodName) {
            var currentPaginationModel = paginationModel.Clone();
            switch (uriType) {
                case ResourceUriType.Next:
                    currentPaginationModel.PageNumber++;
                    break;
                case ResourceUriType.Previous:
                    currentPaginationModel.PageNumber--;
                    break;
                case ResourceUriType.Self:
                    break;
                default:
                    break;
            }
            if (filterConfiguration.SupportsCustomPaginationModel) {
                return linkGenerator.GetPathByAction(methodName, controllerName, TryChangeType(currentPaginationModel, filterConfiguration.CustomPaginationModelType));
            }
            return linkGenerator.GetPathByAction(methodName, controllerName, currentPaginationModel);
        }

        private object TryChangeType(PaginationModel paginationModel, Type newType) {
            object result;
            try {
                result = Convert.ChangeType(paginationModel, newType);
            } catch (InvalidCastException) {
                result = paginationModel;
            }
            return result;
        }
    }
}