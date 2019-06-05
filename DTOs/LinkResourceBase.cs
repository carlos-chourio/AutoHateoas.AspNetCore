using System.Collections.Generic;

namespace CcLibrary.AspNetCore.DTOs {
    public abstract class LinkResourceBase : ILinkResource {
        public IList<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}