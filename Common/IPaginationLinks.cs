namespace AutoHateoas.AspNetCore {
    public interface IPaginationLinks {
        string PreviousPageLink { get; set; }
        string NextPageLink { get; set; }
        string SelfPageLink { get; set; }
    }
}