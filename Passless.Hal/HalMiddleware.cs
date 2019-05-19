using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Passless.Hal.Attributes;
using Passless.Hal.Internal;

namespace Passless.Hal
{
    public class HalMiddleware
    {
        private ILogger<HalMiddleware> logger;
        private RequestDelegate next;
        private IUrlHelperFactory urlHelperFactory;

        public HalMiddleware(
            RequestDelegate next,
            ILogger<HalMiddleware> logger,
            IUrlHelperFactory urlHelperFactory)
        {
            this.next = next
                ?? throw new ArgumentNullException(
                    nameof(next), 
                    "At least the MVC middleware should be after the HAL middleware in the request pipline.");

            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            this.urlHelperFactory = urlHelperFactory
                ?? throw new ArgumentNullException(nameof(urlHelperFactory));
        }

        public async Task Invoke(HttpContext context)
        {
            //var requestFeature = context.Features.Get<IHttpRequestFeature>();
            //var halRequestFeature = new HalHttpRequestFeature(requestFeature);
            //var halContext = new HalHttpContext(context, halRequestFeature);
            //await this.next(halContext);

            context.Items["HalMiddlewareRegistered"] = true;

            await this.next(context);

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

            if (!(actionContext.ActionDescriptor is ControllerActionDescriptor descriptor))
            {
                throw new HalException("Could not establish ControllerActionDescriptor reference.");
            }

            // Set the root resource.
            if (!(halFormattingContext.Result.Value is IResource resource))
            {
                resource = new Resource<object>(halFormattingContext.Result.Value);
            }

            var classAttributes = descriptor.ControllerTypeInfo.GetCustomAttributes<HalEmbedAttribute>(false);
            var methodAttributes = descriptor.MethodInfo.GetCustomAttributes<HalEmbedAttribute>(false);

            var requestFeature = context.Features.Get<IHttpRequestFeature>();
            var urlHelper = urlHelperFactory.GetUrlHelper(halFormattingContext.Context);
            foreach (var halEmbed in classAttributes.Concat(methodAttributes))
            {
                var halRequestFeature = new HalHttpRequestFeature(requestFeature)
                {
                    Path = halEmbed.GetEmbedUri(urlHelper)
                };

                var halContext = new HalHttpContext(context, halRequestFeature);
                halContext.Items["HalMiddlewareRegistered"] = true;

                await this.next(halContext);
                var response = halContext.Response as HalHttpResponse;
                if (response.Resource is IResource embeddedResource)
                {
                    embeddedResource.Rel = halEmbed.Rel;
                    resource.Embedded.Add(embeddedResource);
                }
                else if (response.Resource is IEnumerable enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        resource.Embedded.Add(
                            new Resource<object>(item)
                            {
                                Rel = halEmbed.Rel
                            });
                    }
                }
                else
                {
                    resource.Embedded.Add(
                        new Resource<object>(response.Resource)
                        {
                            Rel = halEmbed.Rel
                        });
                }
            }

            // Now serialize the newly created resource.
            // By executing the modified actionresult.
            halFormattingContext.Result.Value = resource;
            await halFormattingContext.Executor.ExecuteAsync(
                halFormattingContext.Context,
                halFormattingContext.Result);
        }
    }
}
