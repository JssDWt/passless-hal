using System;
using Microsoft.AspNetCore.Mvc;

namespace Passless.AspNetCore.Hal.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class HalEmbedRouteAttribute : HalEmbedAttribute
    {
        public HalEmbedRouteAttribute(string rel, string routeName)
            : base(rel)
        {
            this.RouteName = routeName
                ?? throw new ArgumentNullException(nameof(routeName));
        }

        public string RouteName { get; }

        public override string GetEmbedUri(IUrlHelper url)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            return url.RouteUrl(this.RouteName);
        }
    }
}
