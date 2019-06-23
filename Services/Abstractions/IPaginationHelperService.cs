using AutoHateoas.AspNetCore.Collections.Abstractions;
using AutoHateoas.AspNetCore.Common;

namespace AutoHateoas.AspNetCore.Services.Abstractions {
    public interface IPaginationHelperService<TEntity> {
        PaginationMetadata GeneratePaginationMetaData<T>(IPagedList<T> pagedData, PaginationModel<TEntity> paginationModel, string controllerName, string actionName);
    }
    public interface IPaginationHelperService {
        PaginationMetadata GeneratePaginationMetaData(PaginationInfo paginationInfo, PaginationModel paginationModel, string controllerName, string actionName);
    }
}