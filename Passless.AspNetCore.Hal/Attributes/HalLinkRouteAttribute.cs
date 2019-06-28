using System;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Attributes
{
    /// <summary>
    /// Adds a link to the specified route to the returned resource.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class HalLinkRouteAttribute : HalLinkAttribute, IRouteDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HalLinkRouteAttribute"/> class.
        /// </summary>
        /// <param name="rel">The relation of the linked resource.</param>
        /// <param name="routeName">The name of the route to link to.</param>
        public HalLinkRouteAttribute(string rel, string routeName)
            : base(rel)
        {
            this.RouteName = routeName
                ?? throw new ArgumentNullException(nameof(routeName));
        }

        /// <summary>
        /// Gets the name of the route.
        /// </summary>
        public string RouteName { get; }

        // TODO: Add documentation comment.
        public string Parameter { get; set; }
    }
}
