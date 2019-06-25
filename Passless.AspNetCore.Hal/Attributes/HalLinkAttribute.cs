using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Passless.AspNetCore.Hal.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public abstract class HalLinkAttribute : Attribute, IHalAttribute
    {
        protected HalLinkAttribute(string rel)
        {
            this.Rel = rel
                ?? throw new ArgumentNullException(nameof(rel));
        }

        public string Rel { get; }

        public virtual bool IsSingular { get; set; }
    }
}
