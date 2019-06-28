using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Passless.AspNetCore.Hal.Inspectors;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Extensions
{
    public class HalMiddleware
    {
        private readonly ILogger<HalMiddleware> logger;
        private readonly ResourcePipelineInvokerFactory resourcePipeline;
        private readonly RequestDelegate next;

        public HalMiddleware(
            RequestDelegate next,
            ILogger<HalMiddleware> logger,
            IOptions<HalOptions> options,
            ResourcePipelineInvokerFactory resourcePipeline)
        {
            this.next = next
                ?? throw new ArgumentNullException(
                    nameof(next), 
                    "At least the MVC middleware should be after the HAL middleware in the request pipeline.");

            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            this.resourcePipeline = resourcePipeline
                ?? throw new ArgumentNullException(nameof(resourcePipeline));

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
            var resourcePipelineInvoker = resourcePipeline.Create(new MvcPipeline(this.MvcPipeline));
            ObjectResult result = halFeature.FormattingContext.Result;
            if (resourcePipelineInvoker != null)
            {
                result = await resourcePipelineInvoker.InvokeAsync(halFeature.FormattingContext);
            }

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
