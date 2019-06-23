namespace AutoHateoas.AspNetCore {
    public class PaginationInfo {
        public PaginationInfo(int totalCount, int pageSize, int currentPage, int totalPages) {
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = totalPages;
        }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}