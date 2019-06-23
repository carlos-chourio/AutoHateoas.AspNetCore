using System;
using System.Threading.Tasks;

namespace AutoHateoas.AspNetCore.Attributes {
    public class HateoasResourceAttribute : Attribute {
        public ResourceType ResourceType { get;private set; }

        public HateoasResourceAttribute(ResourceType resourceType) {
            this.ResourceType = resourceType;
        }   
    }

    public enum ResourceType {
        Single,
        Collection
    }
}