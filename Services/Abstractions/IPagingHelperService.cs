using CcLibrary.AspNetCore.Collections.Abstractions;
using CcLibrary.AspNetCore.Common;

namespace CcLibrary.AspNetCore.Services.Abstractions {
    public interface IPagingHelperService {
        PagingMetadata GeneratePagingMetaData<T>(IPagedList<T> pagedData, PagingModel PagingModel, string controllerName, string methodName);
    }
}