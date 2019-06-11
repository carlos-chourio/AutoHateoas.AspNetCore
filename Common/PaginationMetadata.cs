namespace CcLibrary.AspNetCore.Common {
    public class PaginationMetadata {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string PreviousPageLink { get; set; }
        public string NextPageLink { get; set; }
        public string SelfPageLink { get; set; }

        public PaginationMetadata(int totalCount, int pageSize, int currentPage, int totalPages,
            string previousPageLink, string nextPageLink, string selfPageLink) {
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = totalPages;
            PreviousPageLink = previousPageLink;
            NextPageLink = nextPageLink;
            SelfPageLink = selfPageLink;
        }

        public MinipaginationMetadata ToMinipaginationMetadata() {
            return new MinipaginationMetadata() {
                CurrentPage = CurrentPage,
                PageSize = PageSize,
                TotalPages = TotalPages,
                TotalCount = TotalCount
            };
        }

        public class MinipaginationMetadata {
            public int TotalCount { get; set; }
            public int PageSize { get; set; }
            public int CurrentPage { get; set; }
            public int TotalPages { get; set; }
        }
    }
}
