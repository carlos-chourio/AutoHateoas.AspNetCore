using CcLibrary.AspNetCore.Collections.Abstractions;
using CcLibrary.AspNetCore.Common;

namespace CcLibrary.AspNetCore.Services.Abstractions {
    public interface IPagingHelperService<TEntity> {
        PagingMetadata GeneratePagingMetaData<T>(IPagedList<T> pagedData, PagingModel<TEntity> PagingModel, string controllerName, string methodName);
    }
}