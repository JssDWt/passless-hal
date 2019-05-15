using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Passless.Hal
{
    /// <summary>
    /// Interface describing a HAL resource.
    /// </summary>
    public interface IResource
    {
        /// <summary>
        /// Gets the links for the current resource. Containing related links to other resources.
        /// </summary>
        [JsonProperty(Resource.LinksPropertyName)]
        ICollection<ILink> Links { get; set; }

        /// <summary>
        /// Gets embedded resource, accompanied by the current resource, as a full, partial, 
        /// or inconsistent version of the representations served from the target Uri.
        /// </summary>
        [JsonProperty(Resource.EmbeddedPropertyName)]
        ICollection<IEmbeddedResource> Embedded { get; set; }

        /// <summary>
        /// Gets the names of the singular relations, or the relations that always have a single item.
        /// </summary>
        [JsonIgnore]
        ICollection<string> SingularRelations { get; set; }
    }
}
