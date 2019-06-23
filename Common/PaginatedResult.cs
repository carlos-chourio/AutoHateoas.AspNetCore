using Microsoft.AspNetCore.Mvc;

namespace AutoHateoas.AspNetCore.Common {
    public class PaginatedResult : ObjectResult
    {
        public PaginationInfo PaginationInfo { get; private set;}
        public PaginatedResult(object value, PaginationInfo paginationInfo) : base(value)
        {
            PaginationInfo = paginationInfo ?? throw new System.ArgumentNullException(nameof(paginationInfo));
            StatusCode = 200;
        }
    }
}