using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Passless.Hal.Filters;

namespace Passless.Hal.Attributes
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
