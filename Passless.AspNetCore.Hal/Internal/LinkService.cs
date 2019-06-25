using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Passless.AspNetCore.Hal.Attributes;
using Passless.AspNetCore.Hal.Models;

namespace Passless.AspNetCore.Hal.Internal
{
    public class LinkService
    {
        private readonly IUriService<IActionDescriptor> actionService;
        private readonly IUriService<IRouteDescriptor> routeService;
        private readonly IUrlHelperFactory urlHelperFactory;

        public LinkService(
            IUriService<IActionDescriptor> actionService,
            IUriService<IRouteDescriptor> routeService,
            IUrlHelperFactory urlHelperFactory)
        {
            this.actionService = actionService ?? throw new ArgumentNullException(nameof(actionService));
            this.routeService = routeService ?? throw new ArgumentNullException(nameof(routeService));
            this.urlHelperFactory = urlHelperFactory ?? throw new ArgumentNullException(nameof(urlHelperFactory));
        }

        public virtual ICollection<RelUri> GetLinks<TAttribute>(ControllerActionDescriptor descriptor, ActionContext actionContext, object obj)
            where TAttribute : Attribute, IHalAttribute
        {
            var classAttributes = descriptor.ControllerTypeInfo.GetCustomAttributes<TAttribute>(false);
            var methodAttributes = descriptor.MethodInfo.GetCustomAttributes<TAttribute>(false);
            var attributes = classAttributes.Concat(methodAttributes).ToList();

            var urlHelper = urlHelperFactory.GetUrlHelper(actionContext);
            var links = new List<RelUri>();
            foreach (var attribute in attributes)
            {
                string uri = null;
                if (attribute is IActionDescriptor action)
                {
                    uri = actionService.GetUri(action, urlHelper, obj);
                }
                else if (attribute is IRouteDescriptor route)
                {
                    uri = routeService.GetUri(route, urlHelper, obj);
                }

                if (uri != null)
                {
                    links.Add(new RelUri(attribute.Rel, uri, attribute.IsSingular));
                }
            }

            return links;
        }
    }
}
