using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Passless.AspNetCore.Hal.Attributes;
using Passless.AspNetCore.Hal.Internal;
using Passless.AspNetCore.Hal.Models;

namespace Passless.AspNetCore.Hal.Inspectors
{
    public class AttributeLinkInspector : IHalResourceInspector
    {
        private readonly IUrlHelperFactory urlHelperFactory;
        private readonly ILogger<AttributeLinkInspector> logger;
        private readonly LinkService linkService;

        public AttributeLinkInspector(
            IUrlHelperFactory urlHelperFactory,
            ILogger<AttributeLinkInspector> logger,
            LinkService linkService)
        {
            this.urlHelperFactory = urlHelperFactory
                ?? throw new ArgumentNullException(nameof(urlHelperFactory));

            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this.linkService = linkService
                ?? throw new ArgumentNullException(nameof(linkService));
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

            var links = linkService.GetLinks<HalLinkAttribute>(descriptor, context.ActionContext, context.OriginalObject);

            if (context.Resource.Links == null && links.Count > 0)
            {
                context.Resource.Links = new List<ILink>();
            }

            foreach (var link in links)
            {
                var hlink = new Link(link.Rel, link.Uri);
                context.Resource.Links.Add(hlink);
                if (link.IsSingular)
                {
                    if (context.Resource.SingularRelations == null)
                    {
                        context.Resource.SingularRelations = new HashSet<string>();
                    }

                    if (!context.Resource.SingularRelations.Contains(link.Rel))
                    {
                        context.Resource.SingularRelations.Add(link.Rel);
                    }
                }
            }
        }
    }
}
