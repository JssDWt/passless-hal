using System;
namespace Passless.AspNetCore.Hal.Models
{
    /// <summary>
    /// Class representing a link to a resource.
    /// </summary>
    public class Link : ILink
    {
        /// <summary>
        /// Backing field for the Rel property.
        /// </summary>
        private string rel;

        /// <summary>
        /// Backing field for the HRef property.
        /// </summary>
        private string href;

        /// <summary>
        /// Initializes a new instance of the <see cref="Link" /> class.
        /// </summary>
        public Link()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Link" /> class.
        /// </summary>
        /// <param name="href">The location of the resource the link points at.</param>
        public Link(string rel, string href)
            : this()
        {
            this.rel = rel 
                ?? throw new ArgumentNullException(nameof(rel));

            this.href = href
                ?? throw new ArgumentNullException(nameof(href));
        }

        /// <summary>
        /// Gets or sets the relation to the resource.
        /// </summary>
        /// <value>The rel.</value>
        public virtual string Rel 
        {
            get => this.rel; 
            set
            {
                this.rel = value
                    ?? throw new ArgumentNullException(nameof(Rel));
            }
        }

        /// <summary>
        /// Gets or sets the location of the resource the link points at.
        /// </summary>
        public virtual string HRef
        {
            get => this.href;
            set
            {
                this.href = value 
                    ?? throw new ArgumentNullException(nameof(HRef));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the HRef property is a URI template.
        /// </summary>
        public virtual bool? Templated { get; set; }

        /// <summary>
        /// Gets or sets a hint to indicate the media type expected when dereferencing the target resource by the HRef Uri.
        /// </summary>
        public virtual string Type { get; set; }

        /// <summary>
        /// Gets or sets a URL indicating that the link is to be deprecated at a future date.
        /// The URL provides further information about the deprecation.
        /// </summary>
        public virtual string Deprecation { get; set; }

        /// <summary>
        /// Gets or sets a value that may be used as a secondary key for selecting Link Objects which share the same relation type.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets a URI that hints about the profile of the target resource.
        /// </summary>
        public virtual string Profile { get; set; }

        /// <summary>
        /// Gets or sets a human-readable identifier for the link.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets a string that indicates the language of the target resource.
        /// </summary>
        public virtual string HrefLang { get; set; }

        public override string ToString()
        {
            return $"{{\"{nameof(Rel)}\": \"{Rel}\", \"{nameof(HRef)}\": \"{HRef}\"}}";
        }
    }
}
