namespace AutoHateoas.AspNetCore.Common {
    public class PaginationMetadata : PaginationInfo, IPaginationLinks {
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

        public PaginationInfo ToPaginationInfo() {
            return new PaginationInfo() {
                CurrentPage = CurrentPage,
                PageSize = PageSize,
                TotalPages = TotalPages,
                TotalCount = TotalCount
            };
        }
    }
}
