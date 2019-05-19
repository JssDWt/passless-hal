using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Passless.Hal.Filters
{
    public class HalResourceActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            if (!(resultContext.Result is OkObjectResult result))
            {
                return;
            }

            if (!(result.Value is IResource resource))
            {
                resource = new Resource<object>(result.Value);
                result.Value = resource;
            }

            context.HttpContext.Items.Add("HalResource", resource);
        }
    }
}
