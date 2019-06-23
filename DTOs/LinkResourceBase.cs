using System.Collections.Generic;

namespace AutoHateoas.AspNetCore.DTOs {
    public abstract class LinkResourceBase : ILinkResource {
        public IList<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}