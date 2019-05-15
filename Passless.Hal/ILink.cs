using System;
using Newtonsoft.Json;

namespace Passless.Hal
{
    /// <summary>
    /// Interface describing a link.
    /// </summary>
    public interface ILink : IRelated
    {
        /// <summary>
        /// Gets the location of the resource the link points at.
        /// </summary>
        [JsonProperty("href", Required = Required.Always)]
        string HRef { get; }

        /// <summary>
        /// Gets a value indicating whether the HRef property is a URI template.
        /// </summary>
        [JsonProperty("templated", NullValueHandling = NullValueHandling.Ignore)]
        bool? Templated { get; }

        /// <summary>
        /// Gets a hint to indicate the media type expected when dereferencing the target resource by the HRef Uri.
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        string Type { get; }

        /// <summary>
        /// Gets a URL indicating that the link is to be deprecated at a future date.
        /// The URL provides further information about the deprecation.
        /// </summary>
        [JsonProperty("deprecation", NullValueHandling = NullValueHandling.Ignore)]
        string Deprecation { get; }

        /// <summary>
        /// Gets a value that may be used as a secondary key for selecting Link Objects which share the same relation type.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        string Name { get; }

        /// <summary>
        /// Gets a URI that hints about the profile of the target resource.
        /// </summary>
        [JsonProperty("profile", NullValueHandling = NullValueHandling.Ignore)]
        string Profile { get; }

        /// <summary>
        /// Gets a human-readable identifier for the link.
        /// </summary>
        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        string Title { get; }

        /// <summary>
        /// Gets a string that indicates the language of the target resource.
        /// </summary>
        [JsonProperty("hrefLang", NullValueHandling = NullValueHandling.Ignore)]
        string HrefLang { get; }
    }
}
