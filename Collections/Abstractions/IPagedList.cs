using System.Collections.Generic;

namespace CcLibrary.AspNetCore.Collections.Abstractions {
    public interface IPagedList<T> : IList<T> {
        int CurrentPage { get; }
        bool HasNext { get; }
        bool HasPrevious { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
    }
}