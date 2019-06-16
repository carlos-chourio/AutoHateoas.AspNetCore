using System.Collections.Generic;

namespace CcLibrary.AspNetCore.DTOs {
    public class EnvelopCollectionDto<T> {
        public IEnumerable<T> Items { get; set; }
        public IList<LinkDto> Links { get; set; } = new List<LinkDto>();
        public EnvelopCollectionDto(IEnumerable<T> collection) {
            Items = collection;
        }
        public EnvelopCollectionDto() {}
    }

    public class EnvelopDto<TDto> : ILinkResource {
        public TDto Value { get; set; }
        public IList<LinkDto> Links { get; set; } = new List<LinkDto>();
        public EnvelopDto(TDto dto) {
            Value = dto;
        }
        public EnvelopDto() { }
    }
}
