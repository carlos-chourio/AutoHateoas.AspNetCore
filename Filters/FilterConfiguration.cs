using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using AutoHateoas.AspNetCore.Common;
using Microsoft.AspNetCore.Mvc.Routing;
using AutoHateoas.AspNetCore.Attributes;

namespace AutoHateoas.AspNetCore.Filters {
    public class HateoasScanner {

        public HateoasConfiguration HateoasConfiguration { get; set; }
        internal IDictionary<Type, ControllerInfo> ControllerInfoDictionary { get; private set; }

        public HateoasScanner() {
            ControllerInfoDictionary = new Dictionary<Type, ControllerInfo>();
        }

        public void InitializeHateoas(Assembly assembly, HateoasConfiguration hateoasConfiguration) {
            HateoasConfiguration = hateoasConfiguration;
            if (assembly == null) {
                throw new ArgumentNullException(nameof(assembly));
            } else {
                HateoasConfiguration.CurrentAssembly = assembly;
            }
            if (!string.IsNullOrEmpty(HateoasConfiguration.CustomDataType)) {
                HateoasConfiguration.SupportsCustomDataType = true;
            }
            if (HateoasConfiguration.CustomPaginationModelTypes != null && HateoasConfiguration.CustomPaginationModelTypes.Length > 0) {
                foreach (var type in HateoasConfiguration.CustomPaginationModelTypes) {
                    if (type.BaseType != typeof(PaginationModel) || type.BaseType != typeof(PaginationModel<>)) {
                        throw new ArgumentException($"the type {type} doesn't inherit from " +
                            $"{(type.IsGenericType ? typeof(PaginationModel<>).Name : typeof(PaginationModel).Name)}");
                    }
                }
                HateoasConfiguration.SupportsCustomPaginationModel = true;
            }
        }

        /// <summary>
        /// Creates Pagination Profiles by inspectioning the Controllers inside the <paramref name="assembly"/>
        /// </summary>
        /// <param name="assembly">Assembly in which AutoHateoas is going to inspect the code</param>
        /// <param name="customDataType">Custom data type for supporting hateoas</param>
        /// <param name="customPaginationModelTypes">The type of the Custom Pagination Model</param>
        public void ScanControllersInfo() {
            var controllerTypes = HateoasConfiguration.CurrentAssembly.GetTypes().Where(t => t.GetCustomAttribute(typeof(ApiControllerAttribute)) != null);
            Type[] httpMethodTypes = { typeof(HttpGetAttribute), typeof(HttpPostAttribute), typeof(HttpPutAttribute), typeof(HttpPatchAttribute) };
            foreach (var controllerType in controllerTypes) {
                var listOfActions = new List<ActionsByHttpMethodType>();
                foreach (var httpMethodType in httpMethodTypes) {
                    var currentAction = GetAcctionsFromController(controllerType, httpMethodType);
                    if (currentAction!=null) {
                        listOfActions.Add(new ActionsByHttpMethodType(currentAction, httpMethodType));
                    }
                }
                if (listOfActions.Count > 0) {
                    AddControllerInfoToDictionary(listOfActions, controllerType);
                }
            }
        }

        private class ActionsByHttpMethodType {
            public ActionsByHttpMethodType(MemberInfo[] members, Type httpMethodType) {
                Members = members;
                HttpMethodType = httpMethodType;
            }

            public MemberInfo[] Members { get; set; }
            public Type HttpMethodType { get; set; }
        }

        private void AddControllerInfoToDictionary(List<ActionsByHttpMethodType> controllerActions, Type controllerType) {
            var controllerInfo = new ControllerInfo();
            foreach (var action in controllerActions) {
                foreach (var member in action.Members) {
                    try {
                        string httpMethodName = action.HttpMethodType.Name.Replace("Http", "").Replace("Attribute", "");
                        var httpMethodAttribute = (HttpMethodAttribute)member.GetCustomAttribute(action.HttpMethodType);
                        var hateoasResourceAttribute = (HateoasResourceAttribute)member.GetCustomAttribute(typeof(HateoasResourceAttribute));
                        controllerInfo.ControllerActions.Add(new ControllerAction(httpMethodAttribute.Name, hateoasResourceAttribute.ResourceType, httpMethodName, member.Name));
                    } catch (ArgumentNullException argNullEx) {
                        throw new ArgumentNullException(argNullEx.ParamName, $"The {argNullEx.ParamName} property is not set inside {controllerType.Name}");
                    } catch (Exception ex) {
                        throw ex;
                    }
                }
            }
            ControllerInfoDictionary.Add(controllerType, controllerInfo);
        }

        private static MemberInfo[] GetAcctionsFromController(Type controllerType, Type attributeType) {
            var members = controllerType.GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(t => { 
                    var attributes = t.GetCustomAttributes();
                    return attributes.Any(attr => attr.GetType().Equals(attributeType))
                        && attributes.Any(attr => attr.GetType().Equals(typeof(HateoasResourceAttribute)));
                }).ToArray();
            return (members?.Count() != 0) ? members : null;
        }

    }
}