using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Passless.Hal.FeatureFlags;
using Passless.Hal.Formatters;
using Passless.Hal.Internal;

namespace Passless.Hal.Internal
{
    public class HalObjectResultExecutor : IActionResultExecutor<ObjectResult>
    {
        private readonly IActionResultExecutor<ObjectResult> executor;
        private readonly OutputFormatterSelector formatterSelector;
        private readonly Func<Stream, Encoding, TextWriter> writerFactory;
        private readonly ILogger<HalObjectResultExecutor> logger;
        private readonly HalMiddlewareFeatureFlag middlewareFlag;

        public HalObjectResultExecutor(
            IActionResultExecutor<ObjectResult> executor,
            IHttpResponseStreamWriterFactory writerFactory,
            OutputFormatterSelector formatterSelector,
            ILogger<HalObjectResultExecutor> logger,
            HalMiddlewareFeatureFlag middlewareFlag)
        {
            this.executor = executor
                ?? throw new ArgumentNullException(nameof(executor));

            this.formatterSelector = formatterSelector
                ?? throw new ArgumentNullException(nameof(formatterSelector));

            if (writerFactory == null)
            {
                throw new ArgumentNullException(nameof(writerFactory));
            }

            this.writerFactory = writerFactory.CreateWriter;

            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            this.middlewareFlag = middlewareFlag
                ?? throw new ArgumentNullException(nameof(middlewareFlag));
        }

        public Task ExecuteAsync(ActionContext context, ObjectResult result)
        {
            if (context == null
                || context.HttpContext == null
                || context.HttpContext.Items == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            logger.LogDebug("Executing HalObjectResultExecutor.");

            // If this is a hal context, we're adding embedded resources.
            // Just add the resource to the response and don't serialize.
            // The middleware will handle serialization.
            if (context.HttpContext is HalHttpContext)
            {
                logger.LogDebug("HttpContext is a HalHttpContext. Setting response resource.");
                if (context.HttpContext.Response is HalHttpResponse response)
                {
                    response.Resource = result.Value;
                    response.ActionContext = context;
                }

                return Task.CompletedTask;
            }

            // If the HAL middleware is not registered, just run the default executor.
            if (!this.middlewareFlag.IsEnabled)
            {
                logger.LogDebug("Hal middleware is not registered. Running default result executor.");
                return this.executor.ExecuteAsync(context, result);
            }

            var objectType = result.DeclaredType;
            if (objectType == null || objectType == typeof(object))
            {
                objectType = result.Value?.GetType();
            }

            logger.LogTrace("ObjectResult object type is '{0}'", objectType);

            var formatterContext = new OutputFormatterWriteContext(
                context.HttpContext,
                this.writerFactory,
                objectType,
                result.Value);

            // Check if the selected formatter is a HAL formatter
            var selectedFormatter = this.formatterSelector.SelectFormatter(
                formatterContext,
                (IList<IOutputFormatter>)result.Formatters ?? Array.Empty<IOutputFormatter>(),
                result.ContentTypes);
                
            if (selectedFormatter is IHalFormatter)
            {
                logger.LogDebug("Selected an IHalFormatter. Skipping ActionResult execution. That will be handled by the HAL middleware.");

                // Just set the context so the middleware can handle this.
                context.HttpContext.Items["HalFormattingContext"] = new HalFormattingContext
                {
                    Context = context,
                    Result = result,
                    Executor = this.executor
                };

                return Task.CompletedTask;
            }

            logger.LogDebug("Content negotiation did not select an IHalFormatter. Executing default executor instead.");
            // If no hal formatter was selected, just run the default executor.
            return this.executor.ExecuteAsync(context, result);
        }
    }
}
