using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Passless.AspNetCore.Hal.Extensions;
using Passless.AspNetCore.Hal.Factories;
using Passless.AspNetCore.Hal.Inspectors;

namespace Passless.AspNetCore.Hal.Internal
{
    public class ResourcePipelineInvoker
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly ResourceInspectorSelector selector;
        private readonly IHalResourceFactoryMetadata resourceFactory;
        private readonly MvcPipeline mvcPipeline;

        protected ResourcePipelineInvoker()
        {

        }

        public ResourcePipelineInvoker(
            ILoggerFactory loggerFactory,
            ResourceInspectorSelector selector,
            IHalResourceFactoryMetadata resourceFactory,
            MvcPipeline mvcPipeline)
        {
            this.loggerFactory = loggerFactory
                ?? throw new ArgumentNullException(nameof(loggerFactory));

            this.selector = selector
                ?? throw new ArgumentNullException(nameof(selector));

            this.resourceFactory = resourceFactory
                ?? throw new ArgumentNullException(nameof(resourceFactory));

            this.mvcPipeline = mvcPipeline
                ?? throw new ArgumentNullException(nameof(mvcPipeline));
        }

        public virtual async Task<ObjectResult> InvokeAsync(HalFormattingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            IResource rootResource = await ResourcePipeline(context.Context, context.Result.Value, true);
            context.Result.Value = rootResource;
            return context.Result;
        }

        private async Task<IResource> ResourcePipeline(ActionContext actionContext, object resourceObject, bool isRoot)
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
            var inspectors = this.selector.Select(isRoot);

            var resourceInspector = new HalResourceInspectorInvoker(
                inspectors,
                lggr);

            var inspectingContext = new HalResourceInspectingContext(
                resource, actionContext, isRoot, EmbeddedResourceFactory, this.mvcPipeline, resourceObject);

            var result = await resourceInspector.InspectAsync(inspectingContext);
            return result.Resource;
        }

        private Task<IResource> EmbeddedResourceFactory(ActionContext context, object resource)
            => this.ResourcePipeline(context, resource, false);

        private async Task<IResource> InvokeResourceFactory(ResourceFactoryContext context)
        {
            if (this.resourceFactory is IAsyncHalResourceFactory asyncFactory)
            {
                return await asyncFactory.CreateResourceAsync(context);
            }
            else if (this.resourceFactory is IHalResourceFactory syncFactory)
            {
                return syncFactory.CreateResource(context);
            }
            else
            {
                throw new HalException($"Could not understand hal resource factory type. Expecting {nameof(IAsyncHalResourceFactory)} or {nameof(IHalResourceFactory)}.");
            }
        }
    }
}
