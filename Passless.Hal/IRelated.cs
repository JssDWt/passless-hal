using System;
using Newtonsoft.Json;

namespace Passless.Hal
{
    public interface IRelated
    {
        /// <summary>
        /// Gets the relation to the resource/name of the link.
        /// </summary>
        [JsonIgnore]
        string Rel { get; set; }
    }
}
