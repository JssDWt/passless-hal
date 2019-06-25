using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace Passless.AspNetCore.Hal.Internal
{
    public class ValueMapper
    {
        public virtual IDictionary<string, object> GetValues(IReadOnlyDictionary<string, string> mappings, object obj)
        {
            var values = new ExpandoObject() as IDictionary<string, object>;
            if (mappings == null || obj == null || mappings.Count == 0)
            {
                return values;
            }

            var objType = obj.GetType();
            var properties = objType.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
            var useProperties = properties.Join(
                mappings,
                prop => prop.Name,
                param => param.Key,
                (prop, param) => new { Property = prop, Parameter = param.Value });

            foreach (var useProperty in useProperties)
            {
                var propertyValue = useProperty.Property.GetValue(obj);
                values[useProperty.Parameter] = propertyValue;
            }

            return values;
        }
    }
}
