using AutoHateoas.AspNetCore.Common;
using AutoHateoas.AspNetCore.Filters;
using Microsoft.AspNetCore.Builder;
using System;
using System.Reflection;

namespace AutoHateoas.AspNetCore.Extensions {
    public static class AppBuilderExtensions {
        ///// <summary>
        ///// Enables Automatic Hateoas 
        ///// </summary>
        ///// <param name="assembly">The assembly in which you want to support Automatic Hateoas</param>
        ///// <returns></returns>
        //public static IApplicationBuilder UseAutoHateoas(this IApplicationBuilder applicationBuilder, Assembly assembly) {
        //    HateoasScanner filterConfiguration = GetHateoasScanner(applicationBuilder);
        //    filterConfiguration.ScanControllersInfo(assembly);
        //    return applicationBuilder;
        //}

        ///// <summary>
        ///// Enables Automatic Hateoas  with custom data type
        ///// </summary>
        ///// <param name="assembly">The assembly in which Automatic Hateoas will work</param>
        ///// <param name="customDataType">the custom data type to be supported</param>
        ///// <returns>The same instance of <paramref name="applicationBuilder"/></returns>
        //public static IApplicationBuilder UseAutoHateoas(this IApplicationBuilder applicationBuilder, Assembly assembly, string customDataType) {
        //    var filterConfiguration = GetHateoasScanner(applicationBuilder);
        //    filterConfiguration.ScanControllersInfo(assembly, customDataType);
        //    return applicationBuilder;
        //}

        ///// <summary>
        ///// Enables Automatic Hateoas with Custom Pagination Model
        ///// </summary>
        ///// <typeparam name="TPaginationModel">Custom Pagination Model</typeparam>
        ///// <param name="assembly">The assembly in which you want to support Automatic Hateoas</param>
        ///// <returns></returns>
        //public static IApplicationBuilder UseAutoHateoas<TPaginationModel>(this IApplicationBuilder applicationBuilder, Assembly assembly) where TPaginationModel : PaginationModel {
        //    var filterConfiguration = GetHateoasScanner(applicationBuilder);
        //    filterConfiguration.ScanControllersInfo(assembly, null, typeof(TPaginationModel));
        //    return applicationBuilder;
        //}

        ///// <summary>
        ///// Enables Automatic Hateoas  with custom data type and Custom Pagination Model
        ///// </summary>
        ///// <typeparam name="TPaginationModel">Custom Pagination Model</typeparam>
        ///// <param name="assembly">The assembly in which you want to support Automatic Hateoas</param>
        ///// <param name="customDataType">the custom data type you're supporting</param>
        ///// <returns></returns>
        //public static IApplicationBuilder UseAutoHateoas<TPaginationModel>(this IApplicationBuilder applicationBuilder, Assembly assembly, string customDataType) where TPaginationModel : PaginationModel {
        //    var filterConfiguration = GetHateoasScanner(applicationBuilder);
        //    filterConfiguration.ScanControllersInfo(assembly, customDataType, typeof(TPaginationModel));
        //    return applicationBuilder;
        //}

        ///// <summary>
        ///// Enables Automatic Hateoas  with custom data type and Custom Pagination Model
        ///// </summary>
        ///// <param name="paginationModelType">Custom Pagination Model type</param>
        ///// <param name="assembly">The assembly in which you want to support Automatic Hateoas</param>
        ///// <param name="customDataType">the custom data type you're supporting</param>
        ///// <returns></returns>
        //public static IApplicationBuilder UseAutoHateoas(this IApplicationBuilder applicationBuilder, Assembly assembly, string customDataType, Type paginationModelType) {
        //    var filterConfiguration = GetHateoasScanner(applicationBuilder);
        //    filterConfiguration.ScanControllersInfo(assembly, customDataType, paginationModelType);
        //    return applicationBuilder;
        //}

        public static IApplicationBuilder UseAutoHateoas(this IApplicationBuilder applicationBuilder, Assembly assembly, HateoasConfiguration hateoasConfiguration) {
            if (hateoasConfiguration == null) {
                throw new ArgumentNullException(nameof(hateoasConfiguration));
            }
            var filterConfiguration = GetHateoasScanner(applicationBuilder);
            filterConfiguration.InitializeHateoas(assembly, hateoasConfiguration);
            filterConfiguration.ScanControllersInfo();
            return applicationBuilder;
        }

        private static HateoasScanner GetHateoasScanner(IApplicationBuilder applicationBuilder) {
            return applicationBuilder.ApplicationServices.GetService(typeof(HateoasScanner)) as HateoasScanner;
        }
    }
}
