using System;
using Microsoft.AspNetCore.Mvc;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class HalEmbedActionAttribute : HalEmbedAttribute, IActionDescriptor
    {
        public HalEmbedActionAttribute(string rel, string action)
            : base(rel)
        {
            this.Action = action
                ?? throw new ArgumentNullException(nameof(action));
        }


        public string Action { get; }

        public string Controller { get; set; }

        public string Parameter { get; set; }
    }
}
