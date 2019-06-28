using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Passless.AspNetCore.Hal.Attributes;
using Passless.AspNetCore.Hal.Extensions;
using Passless.AspNetCore.Hal.Internal;

namespace Passless.AspNetCore.Hal.Inspectors
{
    public class AttributeEmbedInspector : IAsyncHalResourceInspector
    {
        private readonly IUrlHelperFactory urlHelperFactory;
        private readonly ILogger<AttributeEmbedInspector> logger;
        private readonly LinkService linkService;

        public AttributeEmbedInspector(
            IUrlHelperFactory urlHelperFactory,
            ILogger<AttributeEmbedInspector> logger,
            LinkService linkService)
        {
            this.urlHelperFactory = urlHelperFactory
                ?? throw new ArgumentNullException(nameof(urlHelperFactory));

            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this.linkService = linkService ?? throw new ArgumentNullException(nameof(linkService));
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

            if (context.MvcPipeline == null)
            {
                throw new ArgumentException("Context does not contain the mvc pipeline.", nameof(context));
            }
            
            var requestFeature = context.ActionContext.HttpContext.Features.Get<IHttpRequestFeature>();
            var links = linkService.GetLinks<HalEmbedAttribute>(descriptor, context.ActionContext, context.OriginalObject);

            foreach (var link in links)
            {
                var halRequestFeature = new HalHttpRequestFeature(requestFeature)
                {
                    Method = "GET", 
                    Path = link.Uri
                };

                var halContext = new HalHttpContext(context.ActionContext.HttpContext, halRequestFeature);

                logger.LogDebug("About to invoke MVC pipeline with a GET request on path '{0}'.", link.Uri);
                await context.MvcPipeline.Pipeline(halContext);

                var response = halContext.Response as HalHttpResponse;
                if (response.StatusCode >= 200 && response.StatusCode <= 299)
                {
                    logger.LogDebug("MVC pipeline returned success status code {0}. Invoking HAL resource factory.", response.StatusCode);
                    IResource embedded = await context.EmbeddedResourcePipeline(response.ActionContext, response.Resource);
                    embedded.Rel = link.Rel;

                    if (embedded is IResourceCollection collection)
                    {
                        if (collection.Collection != null)
                        {
                            logger.LogDebug("Embedding collection of {0} resources to rel '{0}'", collection.Collection.Count, link.Rel);
                            foreach (var item in collection.Collection)
                            {
                                item.Rel = link.Rel;
                                context.Resource.Embedded.Add(item);
                            }
                        }
                    }
                    else
                    {
                        logger.LogDebug("Embedding resource to rel '{0}'", link.Rel);
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
