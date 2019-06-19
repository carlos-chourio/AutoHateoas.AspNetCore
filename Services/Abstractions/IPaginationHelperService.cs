using CcLibrary.AspNetCore.Collections.Abstractions;
using CcLibrary.AspNetCore.Common;

namespace CcLibrary.AspNetCore.Services.Abstractions {
    public interface IPaginationHelperService<TEntity> {
        PaginationMetadata GeneratePaginationMetaData<T>(IPagedList<T> pagedData, PaginationModel<TEntity> paginationModel, string controllerName, string actionName);
        PaginationMetadata GeneratePaginationMetaData(PaginationInfo paginationInfo, PaginationModel<TEntity> paginationModel, string controllerName, string actionName);
    }
}