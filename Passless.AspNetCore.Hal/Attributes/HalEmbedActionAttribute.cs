using System;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Attributes
{
    /// <summary>
    /// Embeds the specified controller action result to the returned resource.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class HalEmbedActionAttribute : HalEmbedAttribute, IActionDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HalEmbedActionAttribute"/> class.
        /// </summary>
        /// <param name="rel">The relation of the embedded resource.</param>
        /// <param name="action">The name of the action to invoke.</param>
        public HalEmbedActionAttribute(string rel, string action)
            : base(rel)
        {
            this.Action = action
                ?? throw new ArgumentNullException(nameof(action));
        }

        /// <summary>
        /// Gets the name of the action to embed.
        /// </summary>
        public string Action { get; }

        /// <summary>
        /// Gets or sets the name of the controller to invoke the action on.
        /// </summary>
        public string Controller { get; set; }

        // TODO: Add documentation comment
        public string Parameter { get; set; }
    }
}
