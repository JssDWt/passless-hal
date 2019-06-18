using System;
using Microsoft.AspNetCore.Mvc;

namespace Passless.Hal.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class HalEmbedActionAttribute : HalEmbedAttribute
    {
        public HalEmbedActionAttribute(string rel, string action)
            : base(rel)
        {
            this.Action = action
                ?? throw new ArgumentNullException(nameof(action));
        }


        public string Action { get; }

        public string Controller { get; set; }

        public override string GetEmbedUri(IUrlHelper url)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (this.Controller == null)
            {
                return url.Action(this.Action);
            }

            var result = url.Action(this.Action, this.Controller);
            return result;
        }
    }
}
