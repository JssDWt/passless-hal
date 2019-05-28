using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Passless.Hal
{
    public interface IResourceCollection : IResource
    {
        [JsonIgnore]
        string EmbedRel { get; set; }
        ICollection<IResource> Collection { get; set; }
    }
}
