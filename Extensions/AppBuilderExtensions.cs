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
        public static IApplicationBuilder UseAutomaticHateoas(this IApplicationBuilder applicationBuilder, Assembly assembly) {
            FilterConfiguration filterConfiguration = GetFilterConfiguration(applicationBuilder);
            filterConfiguration.ScanControllersInfo(assembly);
            return applicationBuilder;
        }

        /// <summary>
        /// Enables Automatic Hateoas  with custom data type
        /// </summary>
        /// <param name="assembly">The assembly in which you want to support Automatic Hateoas</param>
        /// <param name="customDataType">the custom data type you're supporting</param>
        /// <returns></returns>
        public static IApplicationBuilder UseAutomaticHateoas(this IApplicationBuilder applicationBuilder, Assembly assembly, string customDataType) {
            var filterConfiguration = GetFilterConfiguration(applicationBuilder);
            filterConfiguration.ScanControllersInfo(assembly, customDataType);
            return applicationBuilder;
        }

        private static FilterConfiguration GetFilterConfiguration(IApplicationBuilder applicationBuilder) {
            return applicationBuilder.ApplicationServices.GetService(typeof(FilterConfiguration)) as FilterConfiguration;
        }
    }
}
