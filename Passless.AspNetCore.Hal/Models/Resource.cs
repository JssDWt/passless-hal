using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Passless.AspNetCore.Hal.Models
{
    /// <summary>
    /// Class describing a resource. Inheriting classes can be serialized as HAL resources.
    /// </summary>
    public abstract class Resource : IResource
    {
        private static ICollection<string> DefaultSingularRelations => new HashSet<string> { "self" };

        public const string LinksPropertyName = "_links";
        public const string EmbeddedPropertyName = "_embedded";
        public static readonly ICollection<string> ReservedProperties = new HashSet<string>
        { 
            LinksPropertyName, 
            EmbeddedPropertyName 
        };

        private string rel = "self";
        private ICollection<ILink> links;
        private ICollection<IResource> embedded;
        private ICollection<string> singularRelations;

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource" /> class.
        /// </summary>
        protected Resource()
            : this(DefaultSingularRelations)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Passless.AspNetCore.Hal.Resource"/> class,
        /// with the specified singular relations.
        /// </summary>
        /// <param name="singularRelations">Singular relations.</param>
        protected Resource(ICollection<string> singularRelations)
            : this(singularRelations, new List<ILink>(), new List<IResource>())
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource" /> class,
        /// using the specified links and embedded resources.
        /// </summary>
        /// <param name="links">The links for the current resource.</param>
        /// <param name="embedded">The embedded resources for the current resource.</param>
        protected Resource(ICollection<ILink> links, ICollection<IResource> embedded)
            : this(DefaultSingularRelations, links, embedded)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Resource" /> class,
        /// using the specified links, embedded resources and singular relations.
        /// </summary>
        /// <param name="links">The links for the current resource.</param>
        /// <param name="embedded">The embedded resources for the current resource.</param>
        protected Resource(
            ICollection<string> singularRelations, 
            ICollection<ILink> links, 
            ICollection<IResource> embedded)
        {
            this.singularRelations = singularRelations
                ?? throw new ArgumentNullException(nameof(singularRelations));
            this.links = links
                ?? throw new ArgumentNullException(nameof(links));
            this.embedded = embedded
                ?? throw new ArgumentNullException(nameof(embedded));
        }

        public string Rel 
        {
            get => this.rel;
            set => this.rel = value
                ?? throw new ArgumentNullException(nameof(Rel));
        }

        /// <summary>
        /// Gets the links for the current resource. Containing related links to other resources.
        /// </summary>
        public ICollection<ILink> Links
        {
            get => this.links;
            set
            {
                this.links = value
                    ?? throw new ArgumentNullException(nameof(Links));
            }
        }

        /// <summary>
        /// Gets embedded resource, accompanied by the current resource, as a full, partial, 
        /// or inconsistent version of the representations served from the target Uri.
        /// </summary>
        public ICollection<IResource> Embedded
        {
            get => this.embedded;
            set
            {
                this.embedded = value
                    ?? throw new ArgumentNullException(nameof(Embedded));
            }
        }

        /// <summary>
        /// Gets the names of the relations that are singular. These relations will not be serialized as an array.
        /// </summary>

        public ICollection<string> SingularRelations 
        {
            get => this.singularRelations;
            set
            {
                this.singularRelations = value
                    ?? throw new ArgumentNullException(nameof(SingularRelations));
            }
        }

        /// <summary>
        /// Determines whether the <see cref="Links"/> property should be serialized.
        /// </summary>
        /// <returns><c>true</c>, if links should be serialized, <c>false</c> otherwise.</returns>
        public virtual bool ShouldSerializeLinks()
        {
            return this.Links != null && this.Links.Count > 0;
        }

        /// <summary>
        /// Determines whether the <see cref="Embedded"/> property should be serialized.
        /// </summary>
        /// <returns><c>true</c>, if embedded should be serialized, <c>false</c> otherwise.</returns>
        public virtual bool ShouldSerializeEmbedded()
        {
            return this.Embedded != null && this.Embedded.Count > 0;
        }
    }
}
