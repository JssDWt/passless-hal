using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Passless.AspNetCore.Hal.Attributes
{
    public class HalLinkActionAttribute : HalLinkAttribute
    {

        public HalLinkActionAttribute(string rel, string action)
            : base(rel)
        {
            this.Action = action
                ?? throw new ArgumentNullException(nameof(action));
        }

        public string Action { get; }

        public string Controller { get; set; }

        public override string GetLinkUri(object obj, IUrlHelper url)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            string result = null;
            var values = new ExpandoObject() as IDictionary<string, object>;
            if (this.Parameters.Count > 0 && obj != null)
            {
                var objType = obj.GetType();
                var properties = objType.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);
                var useProperties = properties.Join(
                    this.Parameters, 
                    prop => prop.Name, 
                    param => param.Key, 
                    (prop, param) => new { Property = prop, Parameter = param.Value });
                    
                foreach (var useProperty in useProperties)
                {
                    var propertyValue = useProperty.Property.GetValue(obj);
                    values[useProperty.Parameter] = propertyValue;
                }
            }


            if (this.Controller == null)
            {
                result = url.Action(this.Action, values);
            }
            else
            {
                result = url.Action(this.Action, this.Controller, values);
            }

            return result;
        }
    }
}
