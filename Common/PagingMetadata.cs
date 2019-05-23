using System;
using CcLibrary.AspNetCore.Validation;

namespace CcLibrary.AspNetCore.Common {
    public class PagingMetadata {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        [PositiveInteger(ErrorMessage = "La pagina actual debe ser un número positivo")]
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string PreviousPageLink { get; set; }
        public string NextPageLink { get; set; }
        public string SelfPageLink { get; set; }
        public PagingMetadata(int totalCount, int pageSize, int currentPage, int totalPages,
            string previousPageLink, string nextPageLink, string selfPageLink) {
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = totalPages;
            PreviousPageLink = previousPageLink;
            NextPageLink = nextPageLink;
            SelfPageLink = selfPageLink;
        }

        public MiniPagingMetadata ToMiniPagingMetadata() {
            return new MiniPagingMetadata() {
                CurrentPage = CurrentPage,
                PageSize = PageSize,
                TotalPages = TotalPages,
                TotalCount = TotalCount
            };
        }

        public class MiniPagingMetadata {
            public int TotalCount { get; set; }
            public int PageSize { get; set; }
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
        }
    }
}
