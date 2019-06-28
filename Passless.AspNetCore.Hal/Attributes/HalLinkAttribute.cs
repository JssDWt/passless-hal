using System;

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

        /// <summary>
        /// Gets the relation of the embedded resource.
        /// </summary>
        public string Rel { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the relation is singular.
        /// </summary>
        public virtual bool IsSingular { get; set; }
    }
}
