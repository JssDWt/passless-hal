using System;
namespace Passless.AspNetCore.Hal.Internal
{
    public class RelUri
    {
        public RelUri(string rel, string uri, bool isSingular)
        {
            this.Rel = rel;
            this.Uri = uri;
            this.IsSingular = isSingular;
        }

        public string Rel { get; }
        public string Uri { get; }
        public bool IsSingular { get; }
    }
}
