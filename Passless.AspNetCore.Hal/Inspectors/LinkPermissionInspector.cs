using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Inspectors
{
    public class LinkPermissionInspector : IAsyncHalResourceInspector
    {
        private readonly ILogger<LinkPermissionInspector> logger;
        public LinkPermissionInspector(ILogger<LinkPermissionInspector> logger)
        {
            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool UseOnEmbeddedResources => true;

        public bool UseOnRootResource => true;

        public async Task<HalResourceInspectedContext> OnResourceInspectionAsync(
            HalResourceInspectingContext context, 
            HalResourceInspectionDelegate next)
        {
            var inspectedContext = await next();
            if (inspectedContext.Resource?.Links == null)
            {
                return inspectedContext;
            }

            var httpContext = context.ActionContext.HttpContext;
            var requestFeature = httpContext.Features.Get<IHttpRequestFeature>();
            var links = inspectedContext.Resource.Links.ToList();

            foreach (var link in links)
            {
                // TODO: Account for external links.
                var halRequestFeature = new HalHttpRequestFeature(requestFeature)
                {
                    Method = "GET",
                    Path = link.HRef
                };

                var linkContext = new LinkValidationHttpContext(httpContext, halRequestFeature);

                // Invoke the mvc pipeline. The LinkValidationFilter will short circuit 
                // the request after the authorization filters have executed.
                await context.MvcPipeline.Pipeline(linkContext);
                var response = linkContext.Response as HalHttpResponse;
                if (response.Resource == LinkValidatedResult.LinkValidatedObject)
                {
                    logger.LogDebug("Client has permissions to access link '{0}'.", link);
                }
                else
                {
                    logger.LogDebug("Client does not have permission to access link '{0}'. Removing link from resource.", link);
                    inspectedContext.Resource.Links.Remove(link);
                }
            }

            return inspectedContext;
        }
    }
}
