using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Internal
{
    public class LinkValidationFilter : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context.HttpContext is LinkValidationHttpContext)
            {
                context.Result = new LinkValidatedResult();
            }
        }
    }
}
