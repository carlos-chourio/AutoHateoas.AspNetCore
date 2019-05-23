using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace CcLibrary.AspNetCore.Extensions {
    public static class IEnumerableExtensions {
        public static IEnumerable<ExpandoObject> ShapeCollectionDataWithRequestedFields<TSource>(this IEnumerable<TSource> collection, string fieldsSeparatedByCommas = null, bool addLinks = false) {
            IList<ExpandoObject> dynamicCollection = new List<ExpandoObject>();
            if (collection != null) {
                List<PropertyInfo> propertyListInfo = ExtensionHelper.GetPropertiesFromObject<TSource>(fieldsSeparatedByCommas, addLinks);
                foreach (TSource element in collection) {
                    ExpandoObject dynamicObject = ExtensionHelper.CreateExpandoObjectFromProperties(element, propertyListInfo);
                    dynamicCollection.Add(dynamicObject);
                }
                return dynamicCollection;
            }
            throw new ArgumentNullException("The collection is empty");
        }
    }
}
