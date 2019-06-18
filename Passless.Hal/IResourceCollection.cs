using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Passless.Hal
{
    public interface IResourceCollection : IResource
    {
        [XmlIgnore]
        [JsonIgnore]
        string EmbedRel { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        ICollection<IResource> Collection { get; set; }
    }
}
