using AutoHateoas.AspNetCore.Collections.Abstractions;
using AutoHateoas.AspNetCore.Common;

namespace AutoHateoas.AspNetCore.Services.Abstractions {
    public interface IPaginationHelperService<TEntity> {
        PaginationMetadata GeneratePaginationMetaData<T>(IPagedList<T> pagedData, PaginationModel paginationModel, string controllerName, string actionName);
        PaginationMetadata GeneratePaginationMetaData(PaginationInfo paginationInfo, PaginationModel paginationModel, string controllerName, string actionName);
    }
}