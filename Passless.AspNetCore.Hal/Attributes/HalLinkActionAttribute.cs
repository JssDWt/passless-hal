using System;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Attributes
{
    /// <summary>
    /// Adds a kink to the specified controller action to the returned resource.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class HalLinkActionAttribute : HalLinkAttribute, IActionDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HalLinkActionAttribute"/> class.
        /// </summary>
        /// <param name="rel">The relation of the embedded resource.</param>
        /// <param name="action">The name of the action to invoke.</param>
        public HalLinkActionAttribute(string rel, string action)
            : base(rel)
        {
            this.Action = action
                ?? throw new ArgumentNullException(nameof(action));
        }

        /// <summary>
        /// Gets the name of the action to link to.
        /// </summary>
        public string Action { get; }

        /// <summary>
        /// Gets or sets the name of the controller the action is linked to.
        /// </summary>
        public string Controller { get; set; }

        // TODO: Add documentation comment
        public string Parameter { get; set; }
    }
}
