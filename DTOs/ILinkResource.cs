using System.Collections.Generic;

namespace CcLibrary.AspNetCore.DTOs {
    public interface ILinkResource {
        IList<LinkDto> Links { get; set; }
    }
}