using System;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Attributes
{
    /// <summary>
    /// Embeds the specified route result to the returned resource.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class HalEmbedRouteAttribute : HalEmbedAttribute, IRouteDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HalEmbedRouteAttribute"/> class.
        /// </summary>
        /// <param name="rel">The relation of the embedded resource.</param>
        /// <param name="routeName">The name of the route to embed the result of.</param>
        public HalEmbedRouteAttribute(string rel, string routeName)
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
