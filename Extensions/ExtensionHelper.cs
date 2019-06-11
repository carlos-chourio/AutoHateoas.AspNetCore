using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace CcLibrary.AspNetCore.Extensions {
    public static class ExtensionHelper {
        internal static List<PropertyInfo> GetPropertiesFromObject<TSource>(string fieldsSeparatedByCommas, bool addLinks = false) {
            var propertyListInfo = new List<PropertyInfo>();
            if (string.IsNullOrEmpty(fieldsSeparatedByCommas)) {
                var tsourceProperties = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                propertyListInfo.AddRange(addLinks
                    ? tsourceProperties
                    : tsourceProperties.Where(t => !t.Name.Equals("Links")).ToArray());
            } else {
                var fieldsArray = fieldsSeparatedByCommas.Split(',');
                foreach (var field in fieldsArray) {
                    string propertyName = field.Trim();
                    var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo != null) {
                        propertyListInfo.Add(propertyInfo);
                    } else {
                        throw new InvalidFilterCriteriaException($"The property {propertyName} could not be found inside {typeof(TSource).Name}");
                    }
                }
                if (addLinks) {
                    var links = typeof(TSource).GetProperty("Links", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (links != null) {
                        propertyListInfo.Add(links);
                    } else {
                        throw new InvalidFilterCriteriaException($"The property Links could not be found inside {typeof(TSource).Name}");
                    }
                }
            }
            return propertyListInfo;
        }
        internal static ExpandoObject CreateExpandoObjectFromProperties<TSource>(TSource source, List<PropertyInfo> propertyListInfo) {
            ExpandoObject dynamicObject = new ExpandoObject();
            foreach (var property in propertyListInfo) {
                ((IDictionary<string, object>)dynamicObject).Add(property.Name,
                    typeof(TSource).GetProperty(property.Name).GetValue(source));
            }
            return dynamicObject;
        }

    }
}
