using System;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class HalLinkRouteAttribute : HalLinkAttribute, IRouteDescriptor
    {
        public HalLinkRouteAttribute(string rel, string routeName)
            : base(rel)
        {
            this.RouteName = routeName
                ?? throw new ArgumentNullException(nameof(routeName));
        }

        public string Parameter { get; set; }

        public string RouteName { get; }
    }
}
