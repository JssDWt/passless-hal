using System;
using Microsoft.AspNetCore.Mvc;

namespace Passless.AspNetCore.Hal.Attributes
{
    /// <summary>
    /// Adds a static link to the returned resource.
    /// </summary>
    public class HalStaticLinkAttribute : HalLinkAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HalStaticLinkAttribute"/> class.
        /// </summary>
        /// <param name="rel">The relation of the linked resource.</param>
        /// <param name="url">The url (href) of the link.</param>
        public HalStaticLinkAttribute(string rel, string url)
            : base (rel)
        {
            this.Url = url
                ?? throw new ArgumentNullException(nameof(url));
        }

        /// <summary>
        /// Gets the Url (HRef) for the link.
        /// </summary>
        public string Url { get; }
    }
}
