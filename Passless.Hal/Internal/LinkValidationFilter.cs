using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Passless.Hal.Internal;

namespace Passless.Hal.Internal
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
