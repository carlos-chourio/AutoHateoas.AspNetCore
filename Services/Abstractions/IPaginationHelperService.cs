using CcLibrary.AspNetCore.Collections.Abstractions;
using CcLibrary.AspNetCore.Common;

namespace CcLibrary.AspNetCore.Services.Abstractions {
    public interface IPaginationHelperService<TEntity> {
        PaginationMetadata GeneratepaginationMetaData<T>(IPagedList<T> pagedData, PaginationModel<TEntity> paginationModel, string controllerName, string methodName);
    }
}