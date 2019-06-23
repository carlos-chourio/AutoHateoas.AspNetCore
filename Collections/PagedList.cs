using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoHateoas.AspNetCore.Collections.Abstractions;

namespace AutoHateoas.AspNetCore.Collections {
    public class PagedList<T> : List<T>, IPagedList<T> {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalCount { get; private set; }
        public int PageSize { get; private set; }
        public bool HasNext {
            get { return CurrentPage < TotalPages; }
        }
        public bool HasPrevious {
            get { return CurrentPage > 1; }
        }

        private PagedList(IEnumerable<T> source, int count, int pageNumber, int pageSize) {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling((double)(TotalCount / pageSize));
            AddRange(source);
        }

        internal static async Task<IPagedList<T>> CreatePagedListAsync(IQueryable<T> source, int pageNumber, int pageSize) {
            int count = source.Count();
            return new PagedList<T>(await source.Skip(pageNumber).Take(pageSize).ToListAsync(), count, pageNumber, pageSize);
        }
    }
}