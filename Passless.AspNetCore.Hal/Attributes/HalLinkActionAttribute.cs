using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Passless.AspNetCore.Hal.Attributes
{
    public class HalLinkActionAttribute : HalLinkAttribute, IActionDescriptor
    {
        public HalLinkActionAttribute(string rel, string action)
            : base(rel)
        {
            this.Action = action
                ?? throw new ArgumentNullException(nameof(action));
        }

        public string Action { get; }

        public string Controller { get; set; }

        public virtual string Parameter { get; set; }
    }
}
