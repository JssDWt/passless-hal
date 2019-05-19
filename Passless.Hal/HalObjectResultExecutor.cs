using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Passless.Hal.Formatters;
using Passless.Hal.Internal;

namespace Passless.Hal
{
    public class HalObjectResultExecutor : IActionResultExecutor<ObjectResult>
    {
        private readonly IActionResultExecutor<ObjectResult> executor;
        private readonly OutputFormatterSelector formatterSelector;
        private readonly Func<Stream, Encoding, TextWriter> writerFactory;

        public HalObjectResultExecutor(
            IActionResultExecutor<ObjectResult> executor,
            IHttpResponseStreamWriterFactory writerFactory,
            OutputFormatterSelector formatterSelector)
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
        }

        public Task ExecuteAsync(ActionContext context, ObjectResult result)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            // If this is a hal context, we're adding embedded resources.
            // Just add the resource to the response and don't serialize.
            // The middleware will handle serialization.
            if (context.HttpContext is HalHttpContext)
            {
                if (context.HttpContext.Response is HalHttpResponse response)
                {
                    response.Resource = result.Value;
                }

                return Task.CompletedTask;
            }

            // If the HAL middleware is not registered, just run the default executor.
            if (!context.HttpContext.Items.TryGetValue("HalMiddlewareRegistered", out object isRegistered))
            {
                return this.executor.ExecuteAsync(context, result);
            }

            var objectType = result.DeclaredType;
            if (objectType == null || objectType == typeof(object))
            {
                objectType = result.Value?.GetType();
            }

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
                // Just set the context so the middleware can handle this.
                context.HttpContext.Items["HalFormattingContext"] = new HalFormattingContext
                {
                    Context = context,
                    Result = result,
                    Executor = this.executor
                };

                return Task.CompletedTask;
            }

            // If no hal formatter was selected, just run the default executor.
            return this.executor.ExecuteAsync(context, result);
        }
    }
}
