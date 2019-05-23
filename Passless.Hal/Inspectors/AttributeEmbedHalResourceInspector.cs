using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Passless.Hal.Extensions;
using Passless.Hal.Internal;

namespace Passless.Hal.Factories
{
    public class AttributeEmbedHalResourceInspector : IAsyncHalResourceInspector
    {
        private readonly IUrlHelperFactory urlHelperFactory;
        private readonly ILogger<AttributeEmbedHalResourceInspector> logger;
        public AttributeEmbedHalResourceInspector(
            IUrlHelperFactory urlHelperFactory,
            ILogger<AttributeEmbedHalResourceInspector> logger)
        {
            this.urlHelperFactory = urlHelperFactory
                ?? throw new ArgumentNullException(nameof(urlHelperFactory));

            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool UseOnEmbeddedResources => false;

        public bool UseOnRootResource => true;

        public async Task<HalResourceInspectedContext> OnResourceInspectionAsync(
            HalResourceInspectingContext context, 
            HalResourceInspectionDelegate next)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (!context.IsRootResource)
            {
                throw new NotSupportedException("This inspector does not support embedded resources.");
            }

            if (context.Resource == null)
            {
                return await next();
            }

            if (context.ActionContext == null)
            {
                throw new ArgumentException("ActionContext cannot be null.", nameof(context));
            }

            if (!(context.ActionContext.ActionDescriptor is ControllerActionDescriptor descriptor))
            {
                throw new HalException("Could not establish ControllerActionDescriptor reference.");
            }

            if (context.ActionContext.HttpContext == null || context.ActionContext.HttpContext.Features == null)
            {
                throw new ArgumentException("HttpContext features cannot be null.", nameof(context));
            }

            var obj = context.ActionContext.HttpContext.RequestServices.GetService(typeof(IActionResultExecutor<ObjectResult>));
            if (!(obj is HalObjectResultExecutor))
            {
                throw new HalException("Cannot execute inspector, because the HalObjectResultExecutor is not registered.");
            }

            if (context.MvcPipeline == null)
            {
                throw new ArgumentException("Context does not contain the mvc pipeline.", nameof(context));
            }

            var classAttributes = descriptor.ControllerTypeInfo.GetCustomAttributes<HalEmbedAttribute>(false);
            var methodAttributes = descriptor.MethodInfo.GetCustomAttributes<HalEmbedAttribute>(false);
            var attributes = classAttributes.Concat(methodAttributes).ToList();

            logger.LogDebug(
                "Found {0} HalEmbed attributes on class '{1}', method '{2}'.",
                attributes.Count,
                descriptor.ControllerTypeInfo,
                descriptor.MethodInfo);

            var requestFeature = context.ActionContext.HttpContext.Features.Get<IHttpRequestFeature>();
            var urlHelper = urlHelperFactory.GetUrlHelper(context.ActionContext);
            foreach (var halEmbed in attributes)
            {
                var path = halEmbed.GetEmbedUri(urlHelper);

                var halRequestFeature = new HalHttpRequestFeature(requestFeature)
                {
                    Method = "GET",
                    Path = path
                };

                var halContext = new HalHttpContext(context.ActionContext.HttpContext, halRequestFeature);

                logger.LogDebug("About to invoke MVC pipeline with a GET request on path '{0}'.", path);
                await context.MvcPipeline(halContext);

                var response = halContext.Response as HalHttpResponse;
                if (response.StatusCode >= 200 && response.StatusCode <= 299)
                {
                    logger.LogDebug("MVC pipeline returned success status code {0}. Invoking HAL resource factory.", response.StatusCode);
                    IResource embedded = await context.ResourceFactory(response.ActionContext, response.Resource);
                    embedded.Rel = halEmbed.Rel;

                    if (embedded is IResourceCollection collection)
                    {
                        if (collection.Collection != null)
                        {
                            logger.LogDebug("Embedding collection of {0} resources to rel '{0}'", collection.Collection.Count, halEmbed.Rel);
                            foreach (var item in collection.Collection)
                            {
                                item.Rel = halEmbed.Rel;
                                context.Resource.Embedded.Add(item);
                            }
                        }
                    }
                    else
                    {
                        logger.LogDebug("Embedding resource to rel '{0}'", halEmbed.Rel);
                        context.Resource.Embedded.Add(embedded);
                    }
                }
                else
                {
                    logger.LogWarning("MVC pipeline returned non-success status code {0}. Ignoring result.", response.StatusCode);
                }
            }

            var result = await next();

            logger.LogTrace("After invoke next.");

            return result;
        }
    }
}
