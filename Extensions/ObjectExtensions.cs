using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace AutoHateoas.AspNetCore.Extensions {
    public static class ObjectExtensions {
        public static ExpandoObject ShapeDataWithRequestedFields<TSource>(this TSource obj, string fieldsSeparatedByCommas = null, bool addLinks = false) where TSource : class {
            if (obj != null) {
                List<PropertyInfo> propertyListInfo = ExtensionHelper.GetPropertiesFromObject<TSource>(fieldsSeparatedByCommas, addLinks);
                return ExtensionHelper.CreateExpandoObjectFromProperties(obj, propertyListInfo);
            }
            throw new ArgumentNullException();
        }
    }
}
