using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Passless.AspNetCore.Hal.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public abstract class HalLinkAttribute : Attribute
    {
        protected Dictionary<string, string> parameters
            = new Dictionary<string, string>();

        protected HalLinkAttribute(string rel)
        {
            this.Rel = rel
                ?? throw new ArgumentNullException(nameof(rel));
        }

        public string Rel { get; }

        private string parameter;
        public virtual string Parameter
        {
            get => this.parameter;
            set
            {
                this.parameter = value;
                this.ParseParameters(value);
            }
        }

        public bool IsSingular { get; set; }

        public IReadOnlyDictionary<string, string> Parameters => this.parameters;

        public abstract string GetLinkUri(object obj, IUrlHelper url);

        protected virtual void ParseParameters(string parameter)
        {
            this.parameters.Clear();
            if (parameter == null)
            {
                return;
            }

            var properties = parameter.Split(',');
            foreach (var property in properties)
            {
                (string objectProperty, string parameterProperty) = ParseProperty(property);
                this.parameters.Add(objectProperty, parameterProperty);
            }
        }

        protected virtual (string, string) ParseProperty(string property)
        {
            var subItems = property.Split('=');
            if (subItems.Length > 1)
            {
                if (subItems.Length > 2)
                {
                    throw new ArgumentException($"Could not understand part '{property}' of {nameof(Parameter)}", nameof(Parameter));
                }

                return (subItems[1], subItems[0]);
            }

            return (subItems[0], subItems[0]);
        }
    }
}
