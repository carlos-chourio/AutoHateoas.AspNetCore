using System;
using System.Collections.Generic;
using AutoHateoas.AspNetCore.Attributes;

namespace AutoHateoas.AspNetCore.Common {
    internal class ControllerInfo {
        internal ICollection<ControllerAction> ControllerActions { get;set; }
        public ControllerInfo() {
            ControllerActions = new List<ControllerAction>();
        }
    }

    internal class ControllerAction {
        public ControllerAction(string httpMethodName, ResourceType resourceType, string methodType, string actionName) {
            if (string.IsNullOrEmpty(httpMethodName)) {
                throw new ArgumentNullException(nameof(httpMethodName));
            }
            if (string.IsNullOrEmpty(methodType)) {
                throw new ArgumentException(nameof(methodType));
            }
            if (string.IsNullOrEmpty(actionName)) {
                throw new ArgumentException(nameof(actionName));
            }
            MethodName = httpMethodName;
            ResourceType = resourceType;
            MethodType = methodType;
            ActionName = actionName;
        }
        internal string MethodName { get;set; }
        internal ResourceType ResourceType { get;set; }
        public string MethodType { get; }
        public string ActionName { get; }
    }
}