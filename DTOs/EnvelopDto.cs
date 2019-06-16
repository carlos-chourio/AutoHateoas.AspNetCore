using System.Collections.Generic;

namespace CcLibrary.AspNetCore.DTOs {
    public class EnvelopCollectionDto<TDto> {
        public IEnumerable<TDto> Items { get; set; }
        public IList<LinkDto> Links { get; set; } = new List<LinkDto>();
        public EnvelopCollectionDto(IEnumerable<TDto> collection) {
            Items = collection;
        }
    }

    public class EnvelopDto<TDto> : ILinkResource {
        public TDto Dto { get; set; }
        public IList<LinkDto> Links { get; set; } = new List<LinkDto>();
        public EnvelopDto(TDto dto) {
            Dto = dto;
        }
    }
}
