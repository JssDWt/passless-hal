using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;

namespace Passless.Hal.Inspectors
{
    public class AddSelfLinkInspector : IHalResourceInspector
    {
        private readonly IUrlHelperFactory urlHelperFactory;
        private readonly ILogger<AddSelfLinkInspector> logger;

        public AddSelfLinkInspector(
            IUrlHelperFactory urlHelperFactory,
            ILogger<AddSelfLinkInspector> logger)
        {
            this.urlHelperFactory = urlHelperFactory
                ?? throw new ArgumentNullException(nameof(urlHelperFactory));

            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool UseOnEmbeddedResources => true;

        public bool UseOnRootResource => true;

        public void OnInspectedResource(HalResourceInspectedContext context)
        {

        }

        public void OnInspectingResource(HalResourceInspectingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Resource == null)
            {
                logger.LogDebug("Resource is null. Nothing to inspect.");
                return;
            }

            if (context.ActionContext == null)
            {
                throw new ArgumentException("ActionContext cannot be null.", nameof(context));
            }

            if (!(context.ActionContext.ActionDescriptor is ControllerActionDescriptor descriptor))
            {
                throw new HalException("Could not establish ControllerActionDescriptor reference.");
            }

            var urlHelper = this.urlHelperFactory.GetUrlHelper(context.ActionContext);
            var classAttributes = descriptor.ControllerTypeInfo.GetCustomAttributes<HalLinkAttribute>(false);
            var methodAttributes = descriptor.MethodInfo.GetCustomAttributes<HalLinkAttribute>(false);
            var attributes = classAttributes.Concat(methodAttributes).ToList();

            logger.LogDebug(
                "Found {0} HalLink attributes on class '{1}', method '{2}'.",
                attributes.Count,
                descriptor.ControllerTypeInfo,
                descriptor.MethodInfo);

            foreach (var halLink in attributes)
            {
                var path = halLink.GetLinkUri(urlHelper);
                var link = new Link(halLink.Rel, path);
            }
        }
    }
}
