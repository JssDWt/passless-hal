using System;
using Microsoft.AspNetCore.Mvc;

namespace Passless.AspNetCore.Hal.Attributes
{
    public class HalStaticLinkAttribute : HalLinkAttribute
    {
        public HalStaticLinkAttribute(string rel, string url)
            : base (rel)
        {
            this.Url = url
                ?? throw new ArgumentNullException(nameof(url));
        }

        public string Url { get; }

        public override string GetLinkUri(object obj, IUrlHelper url) => this.Url;
    }
}
