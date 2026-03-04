using System;
using System.Collections.Generic;
using System.Linq;

namespace StaticClass.Helpers
{
    public static class ReflectionHelpers
    {
        public static object GetPropertyValue(object src, string propName)
        {
            if (src == null) throw new ArgumentException("Value cannot be null.", "src");
            if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

            if (propName.Contains("."))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return GetPropertyValue(GetPropertyValue(src, temp[0]), temp[1]);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                return prop != null ? prop.GetValue(src, null) : null;
            }
        }
        public static Dictionary<string, string> ToDictionary<T>(T obj)
        {
            return typeof(T)
                .GetProperties()
                .Where(p => p.CanRead && p.GetValue(obj) != null)
                .ToDictionary(
                    prop => prop.Name,
                    prop => prop.GetValue(obj)!.ToString()!
                );
        }
    }
}