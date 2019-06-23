using AutoHateoas.AspNetCore.Collections.Abstractions;
using AutoHateoas.AspNetCore.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoHateoas.AspNetCore.Common;
using System.Linq.Dynamic.Core;

namespace AutoHateoas.AspNetCore.Extensions {
    public static class IQueryableExtensions {
        /// <summary>
        /// Creates an IPagedList from an IQuearyable list of Items.
        /// </summary>
        /// <typeparam name="T">The type of the collection item.</typeparam>
        /// <param name="pageSize">The desired page size for Pagination.</param>
        /// <param name="pageNumber">The Number of the page.</param>
        /// <returns></returns>
        public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> items, int pageSize, int pageNumber) {
            return await PagedList<T>.CreatePagedListAsync(items, pageNumber, pageSize);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> items, string orderBySeparatedByCommas, IDictionary<string, PropertyMappingValue> propertyMapping) {
            if (!string.IsNullOrEmpty(orderBySeparatedByCommas)) {
                var orderByValues = orderBySeparatedByCommas.Split(',');
                foreach (var orderByValue in orderByValues.Reverse()) {
                    string trimedOrderBy = orderByValue.Trim();
                    bool isDescending = trimedOrderBy.EndsWith(" desc");
                    string orderByBeforeMapping = (isDescending)
                        ? trimedOrderBy.Substring(0, trimedOrderBy.IndexOf(" desc"))
                        : trimedOrderBy;
                    if (!propertyMapping.ContainsKey(orderByBeforeMapping)) {
                        throw new Exception("The order by clause could not be binded to any Property");
                    }
                    var mapping = propertyMapping[orderByBeforeMapping];
                    foreach (var destinationProperty in mapping.DestinationProperties.Reverse()) {
                        if (mapping.Revert) isDescending = !isDescending;
                        string actualOrderBy = destinationProperty + " " + (isDescending ? "descending" : "ascending");
                        items = items.OrderBy(actualOrderBy);
                    }
                }
            }
            return items;
        }
    }
}