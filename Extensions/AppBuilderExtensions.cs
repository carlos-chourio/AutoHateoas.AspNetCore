using CcLibrary.AspNetCore.Common;
using CcLibrary.AspNetCore.Filters;
using Microsoft.AspNetCore.Builder;
using System.Reflection;

namespace CcLibrary.AspNetCore.Extensions {
    public static class AppBuilderExtensions {
        /// <summary>
        /// Enables Automatic Hateoas 
        /// </summary>
        /// <param name="assembly">The assembly in which you want to support Automatic Hateoas</param>
        /// <returns></returns>
        public static IApplicationBuilder UseAutoHateoas(this IApplicationBuilder applicationBuilder, Assembly assembly) {
            FilterConfiguration filterConfiguration = GetFilterConfiguration(applicationBuilder);
            filterConfiguration.ScanControllersInfo(assembly);
            return applicationBuilder;
        }

        /// <summary>
        /// Enables Automatic Hateoas  with custom data type
        /// </summary>
        /// <param name="assembly">The assembly in which Automatic Hateoas will work</param>
        /// <param name="customDataType">the custom data type to be supported</param>
        /// <returns>The same instance of <paramref name="applicationBuilder"/></returns>
        public static IApplicationBuilder UseAutoHateoas(this IApplicationBuilder applicationBuilder, Assembly assembly, string customDataType) {
            var filterConfiguration = GetFilterConfiguration(applicationBuilder);
            filterConfiguration.ScanControllersInfo(assembly, customDataType);
            return applicationBuilder;
        }

        /// <summary>
        /// Enables Automatic Hateoas with Custom Pagination Model
        /// </summary>
        /// <typeparam name="TPaginationModel">Custom Pagination Model</typeparam>
        /// <param name="assembly">The assembly in which you want to support Automatic Hateoas</param>
        /// <returns></returns>
        public static IApplicationBuilder UseAutoHateoas<TPaginationModel>(this IApplicationBuilder applicationBuilder, Assembly assembly) where TPaginationModel : PaginationModel {
            var filterConfiguration = GetFilterConfiguration(applicationBuilder);
            filterConfiguration.ScanControllersInfo(assembly, null, typeof(TPaginationModel));
            return applicationBuilder;
        }

        /// <summary>
        /// Enables Automatic Hateoas  with custom data type and Custom Pagination Model
        /// </summary>
        /// <typeparam name="TPaginationModel">Custom Pagination Model</typeparam>
        /// <param name="assembly">The assembly in which you want to support Automatic Hateoas</param>
        /// <param name="customDataType">the custom data type you're supporting</param>
        /// <returns></returns>
        public static IApplicationBuilder UseAutoHateoas<TPaginationModel>(this IApplicationBuilder applicationBuilder, Assembly assembly, string customDataType) where TPaginationModel : PaginationModel {
            var filterConfiguration = GetFilterConfiguration(applicationBuilder);
            filterConfiguration.ScanControllersInfo(assembly, customDataType);
            return applicationBuilder;
        }

        private static FilterConfiguration GetFilterConfiguration(IApplicationBuilder applicationBuilder) {
            return applicationBuilder.ApplicationServices.GetService(typeof(FilterConfiguration)) as FilterConfiguration;
        }
    }
}
