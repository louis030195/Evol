using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Evol.Utils
{
    public static class StringExtension
    {
        public static string ReportAllProperties<T>(this T instance) where T : class
        {
 
            if (instance == null)
                return string.Empty;
 
            var strListType = typeof(List<string>);
            var strArrType  = typeof(string[]);
 
            var arrayTypes   = new[] { strListType, strArrType };
            var handledTypes = new[] { typeof(bool), typeof(int), typeof(string), typeof(DateTime), typeof(double), typeof(decimal), typeof(float), strListType, strArrType };
 
            var validProperties = instance.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(prop => handledTypes.Contains(prop.PropertyType))
                .Where(prop => prop.GetValue(instance, null) != null)
                .ToList();
 
            var format = $"{{0,-{validProperties.Max(prp => prp.Name.Length)}}} : {{1}}";
 
            return string.Join(
                Environment.NewLine,
                validProperties.Select(prop => string.Format(format, 
                    prop.Name,
                    arrayTypes.Contains(prop.PropertyType) ? string.Join(", ", (IEnumerable<string>)prop.GetValue(instance, null))
                        : prop.GetValue(instance, null))));
        }
    }
}