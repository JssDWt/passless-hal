using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Passless.AspNetCore.Hal
{
    public interface IResource<T> : IResource where T: class
    {
        [XmlIgnore]
        [JsonIgnore]
        T Data { get; set; }
    }
}
