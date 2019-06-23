using System.Collections.Generic;

namespace AutoHateoas.AspNetCore.Common {
    public class PropertyMappingValue {
        public IEnumerable<string> DestinationProperties { get;set; }
        public bool Revert { get;set; }
        public PropertyMappingValue(IEnumerable<string> destinationProperties, bool revert) {
            DestinationProperties = destinationProperties;
            Revert = revert;
        }
    }
}