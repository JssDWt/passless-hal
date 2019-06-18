using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Passless.AspNetCore.Hal.Internal
{
    public class HalHttpRequestFeature : IHttpRequestFeature
    {
        public HalHttpRequestFeature(IHttpRequestFeature requestFeature)
        {
            if (requestFeature == null)
            {
                throw new ArgumentNullException(nameof(requestFeature));
            }

            this.Protocol = requestFeature.Protocol;
            this.Scheme = requestFeature.Scheme;
            this.Method = requestFeature.Method;
            this.PathBase = requestFeature.PathBase;
            this.Path = requestFeature.Path;
            this.QueryString = requestFeature.QueryString;
            this.RawTarget = requestFeature.RawTarget;
            this.Headers = requestFeature.Headers;
            this.Body = requestFeature.Body;
        }

        public string Protocol { get; set; }
        public string Scheme { get; set; }
        public string Method { get; set; }
        public string PathBase { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string RawTarget { get; set; }
        public IHeaderDictionary Headers { get; set; }
        public Stream Body { get; set; }
    }
}
