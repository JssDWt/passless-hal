using System;
using Microsoft.AspNetCore.Mvc;

namespace Passless.AspNetCore.Hal.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class HalEmbedRouteAttribute : HalEmbedAttribute, IRouteDescriptor
    {
        public HalEmbedRouteAttribute(string rel, string routeName)
            : base(rel)
        {
            this.RouteName = routeName
                ?? throw new ArgumentNullException(nameof(routeName));
        }

        public string RouteName { get; }

        public string Parameter { get; set; }
    }
}
