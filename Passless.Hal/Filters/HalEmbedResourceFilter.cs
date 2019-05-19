using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Passless.Hal.Attributes;

namespace Passless.Hal.Filters
{
    public class HalEmbedResourceFilter : IAsyncResourceFilter
    {
        private readonly IUrlHelperFactory urlHelperFactory;

        public HalEmbedResourceFilter(IUrlHelperFactory urlHelperFactory)
        {
            this.urlHelperFactory = urlHelperFactory
                ?? throw new ArgumentNullException(nameof(urlHelperFactory));
        }

        public Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            return next();
            //if (context == null) throw new ArgumentNullException(nameof(context));
            //if (next == null) throw new ArgumentNullException(nameof(next));

            //// Make sure a filter lower in the pipeline will transform objects to IResource objects.
            //var resourceFilter = context.Filters.FirstOrDefault((filter) => filter is HalResourceActionFilter);

            //// Make sure the controller and action can be determined.
            //var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            //var controllerType = controllerActionDescriptor?.ControllerTypeInfo;
            //var actionMethodInfo = controllerActionDescriptor?.MethodInfo;

            //// Make sure we don't execute the code for embedded resources.
            //context.HttpContext.Items.TryGetValue("IsHalContext", out object isHalContextObject);

            //if (resourceFilter == null
            //    || controllerActionDescriptor == null
            //    || controllerType == null
            //    || actionMethodInfo == null
            //    || (isHalContextObject != null && ((bool)isHalContextObject)))
            //{
            //    await next();
            //    return;
            //}

            //var classAttributes = controllerType.GetCustomAttributes<HalEmbedAttribute>(false);
            //var methodAttributes = actionMethodInfo.GetCustomAttributes<HalEmbedAttribute>(false);

            //var router = context.RouteData.Routers.FirstOrDefault();

            //var resultContext = await next();
            //if (!(resultContext.Result is OkObjectResult result
            //    && result.Value is IResource resource))
            //{
            //    return;
            //}

            //var toEmbed = new List<IResource>();
            //var urlHelper = urlHelperFactory.GetUrlHelper(context);
            //PathString oldPath = context.HttpContext.Request.Path;

            //context.HttpContext.Items.Add("IsHalContext", true);

            //try
            //{
            //    foreach (var halEmbed in classAttributes.Concat(methodAttributes))
            //    {
                   
            //        var newHttpContext = new DefaultHttpContext(context.HttpContext.Features);

            //        newHttpContext.Items.Add("IsHalContext", true);
            //        newHttpContext.Request.Cookies = context.HttpContext.Request.Cookies;
            //        newHttpContext.Request.Host = context.HttpContext.Request.Host;
            //        newHttpContext.Request.IsHttps = context.HttpContext.Request.IsHttps;
            //        newHttpContext.Request.Method = "GET";

            //        string url = halEmbed.GetEmbedUri(urlHelper);
            //        newHttpContext.Request.Path = PathString.FromUriComponent(url);
            //        newHttpContext.Request.PathBase = context.HttpContext.Request.PathBase;
            //        newHttpContext.Request.Protocol = context.HttpContext.Request.Protocol;
            //        newHttpContext.Request.Query = context.HttpContext.Request.Query;
            //        newHttpContext.Request.QueryString = context.HttpContext.Request.QueryString;
            //        newHttpContext.Request.Scheme = context.HttpContext.Request.Scheme;

            //        var newRouteContext = new RouteContext(newHttpContext);
            //        newRouteContext.RouteData.Routers.Add(router);
            //        newHttpContext.Response.Body = Stream.Null;
            //        await router.RouteAsync(newRouteContext);

            //        if (newRouteContext.Handler != null)
            //        {
            //            newHttpContext.Features[typeof(IRoutingFeature)] = new RoutingFeature()
            //            {
            //                RouteData = newRouteContext.RouteData,
            //            };

            //            await newRouteContext.Handler(newHttpContext);

            //            if (newHttpContext.Response)
            //            embedContexts.Add(new Tuple<string, HttpContext>(halEmbed.Rel, newHttpContext));
            //        }
            //    }

            //    var resultContext = await nextTask;

            //    if (resultContext.Result is OkObjectResult result
            //        && result.Value is IResource resource)
            //    {
            //        await Task.WhenAll(embedTasks);

            //        foreach (var embedContext in embedContexts)
            //        {
            //            embedContext.Item2.Items.TryGetValue("HalResource", out object resourceObject);

            //            if (resourceObject is IResource embedResource)
            //            {
            //                embedResource.Rel = embedContext.Item1;
            //                resource.Embedded.Add(embedResource);
            //            }
            //        }
            //    }

            //    context.HttpContext.Items["IsHalContext"] = false;
            //}
            //catch (Exception ex)
            //{
            //    string hello = "";
            //}
            //finally
            //{
            //    try
            //    {
            //        await Task.WhenAll(embedTasks);
            //    }
            //    catch
            //    {
            //        // squelch
            //    }
            //}
        }
    }
}
