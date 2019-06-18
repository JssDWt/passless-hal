using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Passless.AspNetCore.Hal.Internal
{
    public class LinkValidationHttpContext : HalHttpContext
    {
        public LinkValidationHttpContext(HttpContext context, IHttpRequestFeature requestFeature) 
            : base(context, requestFeature)
        {
        }
    }
}
