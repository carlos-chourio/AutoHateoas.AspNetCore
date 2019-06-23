using System.Collections.Generic;

namespace AutoHateoas.AspNetCore.DTOs {
    public class EnvelopCollection<T> {
        public IEnumerable<T> Items { get; set; }
        public IList<LinkDto> Links { get; set; } = new List<LinkDto>();
        public EnvelopCollection(IEnumerable<T> collection) {
            Items = collection;
        }
        public EnvelopCollection() {}
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
