using System.Collections.Generic;
using CcLibrary.AspNetCore.Attributes;

namespace CcLibrary.AspNetCore.Common {
    internal class ControllerInfo {
        internal ICollection<ControllerInfoValue> ControllerInfoValues { get;set; }
        public ControllerInfo() {
            ControllerInfoValues = new List<ControllerInfoValue>();
        }
    }

    internal class ControllerInfoValue {
        public ControllerInfoValue(string methodName, ResourceType resourceType) {
            if (string.IsNullOrEmpty(methodName)) {
                throw new System.ArgumentException("message", nameof(methodName));
            }
            MethodName = methodName;
            ResourceType = resourceType;
        }
        internal string MethodName { get;set; }
        internal ResourceType ResourceType { get;set; }
    }
}