using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Passless.Hal.Models;

namespace Passless.Hal
{
    /// <summary>
    /// Interface describing a HAL resource.
    /// </summary>
    [XmlRoot(ElementName = Constants.ResourceElementName, Namespace = Constants.HalNamespace)]
    public interface IResource : IRelated
    {
        /// <summary>
        /// Gets the links for the current resource. Containing related links to other resources.
        /// </summary>
        [XmlElement(ElementName = Constants.LinkElementName, Namespace = Constants.HalNamespace)]
        [JsonProperty(Resource.LinksPropertyName)]
        ICollection<ILink> Links { get; set; }

        /// <summary>
        /// Gets embedded resource, accompanied by the current resource, as a full, partial, 
        /// or inconsistent version of the representations served from the target Uri.
        /// </summary>
        [XmlElement(ElementName = Constants.ResourceElementName, Namespace = Constants.HalNamespace)]
        [JsonProperty(Resource.EmbeddedPropertyName)]
        ICollection<IResource> Embedded { get; set; }

        /// <summary>
        /// Gets the names of the singular relations, or the relations that always have a single item.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        ICollection<string> SingularRelations { get; set; }
    }
}
