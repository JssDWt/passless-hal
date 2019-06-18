using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Passless.Hal
{
    public interface IRelated
    {
        /// <summary>
        /// Gets the relation to the resource/name of the link.
        /// </summary>
        [XmlAttribute(AttributeName = Constants.RelAttributeName, DataType = "string", Namespace = Constants.HalNamespace)]
        [JsonIgnore]
        string Rel { get; set; }
    }
}
