using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Passless.Hal.Internal;

namespace Passless.Hal.Extensions
{
    public class HalMiddleware
    {
        private ILogger<HalMiddleware> logger;
        public RequestDelegate Next { get; }
        private IUrlHelperFactory urlHelperFactory;
        private HalOptions halOptions;

        public HalMiddleware(
            RequestDelegate next,
            ILogger<HalMiddleware> logger,
            IUrlHelperFactory urlHelperFactory,
            IOptions<HalOptions> halOptions)
        {
            this.Next = next
                ?? throw new ArgumentNullException(
                    nameof(next), 
                    "At least the MVC middleware should be after the HAL middleware in the request pipline.");

            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            this.urlHelperFactory = urlHelperFactory
                ?? throw new ArgumentNullException(nameof(urlHelperFactory));

            this.halOptions = halOptions?.Value
                ?? throw new ArgumentNullException(nameof(halOptions));
        }

        public async Task Invoke(HttpContext context)
        {
            //var requestFeature = context.Features.Get<IHttpRequestFeature>();
            //var halRequestFeature = new HalHttpRequestFeature(requestFeature);
            //var halContext = new HalHttpContext(context, halRequestFeature);
            //await this.next(halContext);

            context.Items["HalMiddlewareRegistered"] = true;

            await this.Next(context);

            if (!context.Items.TryGetValue("HalFormattingContext", out object formatObject)
                ||!(formatObject is HalFormattingContext halFormattingContext))
            {
                logger.LogDebug("Hal formatting context not found, other formatters are handling this response.");
                return;
            }


            if (context.Response.HasStarted)
            {
                logger.LogWarning("Could not transform response into a HAL response, because the response has already started.");
                return;
            }

            // Apparently we're dealing with a HAL response now. 

            var actionContext = halFormattingContext.Context;
            if (actionContext == null)
            {
                throw new HalException("Could not establish actionContext reference.");
            }

            var objectResult = halFormattingContext.Result;
            if (objectResult == null)
            {
                throw new HalException("Could not establish objectResult reference.");
            }

            // Allow resourcefactories to modify the resource.
            if (this.halOptions.ResourceFactories != null)
            {
                foreach (var resourceFactory in this.halOptions.ResourceFactories.Where(f => f != null))
                {
                    if (resourceFactory is IAsyncHalResourceFactory asyncFactory)
                    {
                        await asyncFactory.CreateResourceAsync(this, actionContext, objectResult);
                    }
                    else if (resourceFactory is IHalResourceFactory syncFactory)
                    {
                        syncFactory.CreateResource(this, actionContext, objectResult);
                    }
                    else
                    {
                        throw new HalException("Could not understand hal resource factory type. Expecting IAsyncHalResourceFactory or IHalResourceFactory.");
                    }

                    if (context.Response.HasStarted)
                    {
                        logger.LogWarning("Invoked ResourceFactory of type '{0}' started writing to the response. Cancelling further processing.", resourceFactory.GetType().FullName);
                        return;
                    }
                }
            }

            // Now serialize the newly created resource.
            // By executing the modified actionresult.
            await halFormattingContext.Executor.ExecuteAsync(
                actionContext,
                objectResult);
        }
    }
}
