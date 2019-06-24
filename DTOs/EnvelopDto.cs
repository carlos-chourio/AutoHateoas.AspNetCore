using System.Collections.Generic;

namespace AutoHateoas.AspNetCore.DTOs {
    public class EnvelopCollection<T> {
        public IEnumerable<T> Values { get; set; }
        public IList<LinkDto> Links { get; set; } = new List<LinkDto>();
        public EnvelopCollection(IEnumerable<T> collection) {
            Values = collection;
        }
        public EnvelopCollection() {}
    }

    public class EnvelopDto<T>  {
        public T Value { get; set; }
        public IList<LinkDto> Links { get; set; } = new List<LinkDto>();
        public EnvelopDto(T value) {
            Value = value;
        }
        public EnvelopDto(T value, IList<LinkDto> links) {
            Value = value;
            Links = links;
        }
    }
}
