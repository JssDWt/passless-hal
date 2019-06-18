﻿using System;
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
        private readonly IUrlHelperFactory urlHelperFactory;
        private readonly HalOptions halOptions;
        private readonly ILoggerFactory loggerFactory;
        public HalMiddleware(
            RequestDelegate next,
            ILogger<HalMiddleware> logger,
            IUrlHelperFactory urlHelperFactory,
            IOptions<HalOptions> halOptions,
            ILoggerFactory loggerFactory)
        {
            this.next = next
                ?? throw new ArgumentNullException(
                    nameof(next), 
                    "At least the MVC middleware should be after the HAL middleware in the request pipeline.");

            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            this.urlHelperFactory = urlHelperFactory
                ?? throw new ArgumentNullException(nameof(urlHelperFactory));

            this.halOptions = halOptions?.Value
                ?? throw new ArgumentNullException(
                    nameof(halOptions),
                    "Be sure to add 'AddHal' to the 'ConfigureServices' method in the Startup class.");

            this.loggerFactory = loggerFactory
                ?? throw new ArgumentNullException(nameof(loggerFactory));
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
            var actionContext = halFeature.FormattingContext.Context;
            if (actionContext == null)
            {
                throw new HalException("Could not establish actionContext reference.");
            }

            var objectResult = halFeature.FormattingContext.Result;
            if (objectResult == null)
            {
                throw new HalException("Could not establish objectResult reference.");
            }

            IResource rootResource = await ResourceFactory(actionContext, objectResult.Value, true);
            objectResult.Value = rootResource;

            if (context.Response.HasStarted)
            {
                logger.LogWarning("Response had already started before executing the formatter. Skipping formatter.");
                return;
            }

            // Now serialize the newly created resource.
            // By executing the modified actionresult.
            await halFeature.FormattingContext.Executor.ExecuteAsync(
                actionContext,
                objectResult);
        }

        private async Task<IResource> ResourceFactory(ActionContext actionContext, object resourceObject, bool isRoot)
        {
            // TODO: Should the resourceObject actually be an objectresult?
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            var resourceFactoryContext = new ResourceFactoryContext
            {
                ActionContext = actionContext,
                Resource = resourceObject,
                IsRootResource = isRoot
            };

            var resource = await InvokeResourceFactory(resourceFactoryContext);

            var lggr = this.loggerFactory.CreateLogger<HalResourceInspectorInvoker>();
            var inspectors = this.halOptions.ResourceInspectors.Where(i => i != null);
            if (isRoot)
            {
                inspectors = inspectors.Where(i => i.UseOnRootResource);
            }
            else
            {
                inspectors = inspectors.Where(i => i.UseOnEmbeddedResources);
            }

            var resourceInspector = new HalResourceInspectorInvoker(
                inspectors.ToArray(),
                lggr);

            var inspectingContext = new HalResourceInspectingContext(
                resource, actionContext, isRoot, EmbeddedResourceFactory, this.MvcPipeline, resourceObject);

            var result = await resourceInspector.InspectAsync(inspectingContext);
            return result.Resource; 
        }

        private Task<IResource> EmbeddedResourceFactory(ActionContext context, object resource)
            => this.ResourceFactory(context, resource, false);

        private async Task<IResource> InvokeResourceFactory(ResourceFactoryContext context)
        {
            IResource resource = null;

            if (this.halOptions.ResourceFactory is IAsyncHalResourceFactory asyncFactory)
            {
                resource = await asyncFactory.CreateResourceAsync(context);
            }
            else if (this.halOptions.ResourceFactory is IHalResourceFactory syncFactory)
            {
                resource = syncFactory.CreateResource(context);
            }
            else
            {
                throw new HalException("Could not understand hal resource factory type. Expecting IAsyncHalResourceFactory or IHalResourceFactory.");
            }

            return resource;
        }

        private async Task MvcPipeline(HttpContext context)
        {
            this.logger.LogDebug("Start HAL Resource inspector invoking MVC pipeline.");
            await this.next(context);
            this.logger.LogDebug("End HAL Resource inspector invoking MVC pipeline.");
        }
    }
}