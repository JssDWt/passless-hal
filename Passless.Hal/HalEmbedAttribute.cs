using System;
using Microsoft.AspNetCore.Mvc;

namespace Passless.Hal
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public abstract class HalEmbedAttribute : Attribute
    {
        protected HalEmbedAttribute(string rel)
        {
            this.Rel = rel
                ?? throw new ArgumentNullException(nameof(rel));
        }

        public string Rel { get; }

        public abstract string GetEmbedUri(IUrlHelper url);
    }
}
