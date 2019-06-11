using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CcLibrary.AspNetCore.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CcLibrary.AspNetCore.Filters {
    public class FilterConfiguration {
        public IDictionary<Type, LinkDto[]> PaginationProfiles { get; set; }
        //private readonly ILogger logger;
        private string dumbLogger;
        internal bool SupportsCustomDataType { get; private set; }
        internal string CustomDataType { get; private set; }

        public FilterConfiguration() {
            //this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            PaginationProfiles = new Dictionary<Type, LinkDto[]>();
        }

        public void CreatePaginationProfiles(Assembly assembly, string customDataType=null) {
            if (assembly == null) {
                throw new ArgumentNullException(nameof(assembly));
            }
            if (!string.IsNullOrEmpty(customDataType)) {
                SupportsCustomDataType = true;
                CustomDataType = customDataType;
            }
            var controllerTypes = assembly.GetTypes().Where(t => t.GetCustomAttribute(typeof(ApiControllerAttribute)) != null);
            Type[] attributeTypes = { typeof(HttpGetAttribute), typeof(HttpPostAttribute), typeof(HttpPutAttribute), typeof(HttpPatchAttribute) };
            foreach (var controllerType in controllerTypes) {
                //logger.LogInformation($"Creating Profile for Controller: {controllerType.Name}", null);
                foreach (var attributeType in attributeTypes) {
                    MemberInfo[] actions = GetAcctionsFromController(controllerType, attributeType);
                    AddActionsToPaginationProfile(actions, controllerType, attributeType);
                }
            }
        }

        private void AddActionsToPaginationProfile(MemberInfo[] actions, Type controllerType, Type attributeType) {
            string controllerName = controllerType.Name;
            string attributeName = attributeType.Name;
            foreach (var action in actions) {
                dumbLogger += $"{controllerName} - {attributeName.Replace("Attribute", "")} - {action}";
            }
            dumbLogger += Environment.NewLine;
        }

        private static MemberInfo[] GetAcctionsFromController(Type controllerType, Type attributeType) {
            return controllerType.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(t => t.GetCustomAttributes().Any(y => y.GetType().Equals(attributeType))).ToArray();
        }

    }
}