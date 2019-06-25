using System;
using Microsoft.AspNetCore.Mvc;

namespace Passless.AspNetCore.Hal.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public abstract class HalEmbedAttribute : Attribute, IHalAttribute
    {
        protected HalEmbedAttribute(string rel)
        {
            this.Rel = rel
                ?? throw new ArgumentNullException(nameof(rel));
        }

        public string Rel { get; }

        public bool IncludeLink { get; set; }

        public bool IsSingular { get; set; }
    }
}
