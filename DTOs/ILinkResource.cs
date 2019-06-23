using System.Collections.Generic;

namespace AutoHateoas.AspNetCore.DTOs {
    public interface ILinkResource {
        IList<LinkDto> Links { get; set; }
    }
}