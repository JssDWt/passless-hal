using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Passless.Hal.Extensions;
using Passless.Hal.Internal;

namespace Passless.Hal.Factories
{
    public class AttributeEmbedHalResourceFactory : IAsyncHalResourceFactory
    {
        private readonly IUrlHelperFactory urlHelperFactory;
        public AttributeEmbedHalResourceFactory(
            IUrlHelperFactory urlHelperFactory)
        {
            this.urlHelperFactory = urlHelperFactory
                ?? throw new ArgumentNullException(nameof(urlHelperFactory));
        }

        public async Task CreateResourceAsync(HalMiddleware middleware, ActionContext actionContext, ObjectResult objectResult)
        {
            if (middleware == null)
            {
                throw new ArgumentNullException(nameof(middleware));
            }

            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            if (objectResult == null)
            {
                throw new ArgumentNullException(nameof(objectResult));
            }

            if (!(actionContext.ActionDescriptor is ControllerActionDescriptor descriptor))
            {
                throw new HalException("Could not establish ControllerActionDescriptor reference.");
            }

            if (actionContext.HttpContext == null || actionContext.HttpContext.Features == null)
            {
                throw new ArgumentException("HttpContext features cannot be null.", nameof(actionContext));
            }

            // Set the root resource.
            if (!(objectResult.Value is IResource resource))
            {
                resource = new Resource<object>(objectResult.Value);
            }

            var classAttributes = descriptor.ControllerTypeInfo.GetCustomAttributes<HalEmbedAttribute>(false);
            var methodAttributes = descriptor.MethodInfo.GetCustomAttributes<HalEmbedAttribute>(false);

            var requestFeature = actionContext.HttpContext.Features.Get<IHttpRequestFeature>();
            var urlHelper = urlHelperFactory.GetUrlHelper(actionContext);
            foreach (var halEmbed in classAttributes.Concat(methodAttributes))
            {
                var halRequestFeature = new HalHttpRequestFeature(requestFeature)
                {
                    Path = halEmbed.GetEmbedUri(urlHelper)
                };

                var halContext = new HalHttpContext(actionContext.HttpContext, halRequestFeature);
                halContext.Items["HalMiddlewareRegistered"] = true;

                await middleware.Next(halContext);
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

            objectResult.Value = resource;
        }
    }
}
