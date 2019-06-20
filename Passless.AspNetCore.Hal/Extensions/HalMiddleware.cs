using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Passless.AspNetCore.Hal.Factories;
using Passless.AspNetCore.Hal.FeatureFlags;
using Passless.AspNetCore.Hal.Inspectors;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Extensions
{
    // TODO: Build an inspector that checks the permissions for a link, by invoking all filters.
    public class HalMiddleware
    {
        private readonly ILogger<HalMiddleware> logger;
        private RequestDelegate next;

        public HalMiddleware(
            RequestDelegate next,
            ILogger<HalMiddleware> logger,
            IOptions<HalOptions> options)
        {
            this.next = next
                ?? throw new ArgumentNullException(
                    nameof(next), 
                    "At least the MVC middleware should be after the HAL middleware in the request pipeline.");

            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            if (options == null
                || options.Value == null)
            {
                throw new ArgumentException("No HalOptions found. Make sure 'AddHal' is added in the ConfigureServices method.");
            }
        }

        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Features == null)
            {
                throw new ArgumentException("Features cannot be null.", nameof(context));
            }

            var halFeature = new HalFeature();
            context.Features[typeof(HalFeature)] = halFeature;

            await this.next(context);

            if (halFeature.FormattingContext == null)
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
            // Invoke the resource factory.
            var resourceFactoryInvoker = ActivatorUtilities.CreateInstance<ResourceFactoryInvoker>(
                context.RequestServices,
                new MvcPipeline(this.MvcPipeline));

            var result = await resourceFactoryInvoker.InvokeAsync(halFeature.FormattingContext);

            if (context.Response.HasStarted)
            {
                logger.LogWarning("Response had already started before executing the formatter. Skipping formatter.");
                return;
            }

            // Now serialize the newly created resource.
            // By executing the modified actionresult.
            await halFeature.FormattingContext.Executor.ExecuteAsync(
                halFeature.FormattingContext.Context,
                result);
        }

        private async Task MvcPipeline(HttpContext context)
        {
            this.logger.LogDebug("Start HAL Resource inspector invoking MVC pipeline.");
            await this.next(context);
            this.logger.LogDebug("End HAL Resource inspector invoking MVC pipeline.");
        }
    }
}
