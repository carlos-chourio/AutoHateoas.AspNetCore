using System.Collections.Generic;

namespace CcLibrary.AspNetCore.DTOs {
    public class EnvelopDto<TDto> /*where TDto : IIdentityDto */{
        public IEnumerable<TDto> Items { get; set; }
        public IList<LinkDto> Links { get; set; } = new List<LinkDto>();
        public EnvelopDto(IEnumerable<TDto> collection) {
            Items = collection;
        }
    }
}
