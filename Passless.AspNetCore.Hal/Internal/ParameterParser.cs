using System;
using System.Collections.Generic;

namespace Passless.AspNetCore.Hal.Internal
{
    public class ParameterParser
    {
        private Dictionary<string, Dictionary<string, string>> parameterCache
            = new Dictionary<string, Dictionary<string, string>>();

        public virtual IReadOnlyDictionary<string, string> Parse(string parameter)
        {
            // Try to get the parameter from cache first.
            if (parameterCache.TryGetValue(parameter, out Dictionary<string, string> parameters))
            {
                return parameters;
            }

            parameters = new Dictionary<string, string>();
            if (parameter == null)
            {
                return parameters;
            }

            var properties = parameter.Split(',');
            foreach (var property in properties)
            {
                (string objectProperty, string parameterProperty) = ParseProperty(property);
                parameters.Add(objectProperty, parameterProperty);
            }

            parameterCache[parameter] = parameters;
            return parameters;
        }

        protected virtual (string, string) ParseProperty(string property)
        {
            var subItems = property.Split('=');
            if (subItems.Length > 1)
            {
                if (subItems.Length > 2)
                {
                    throw new ArgumentException($"Could not understand property '{property}'");
                }

                return (subItems[1], subItems[0]);
            }

            return (subItems[0], subItems[0]);
        }
    }
}
