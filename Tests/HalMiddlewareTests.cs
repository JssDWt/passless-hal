using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Passless.AspNetCore.Hal.Extensions;
using Passless.AspNetCore.Hal.Factories;
using Passless.AspNetCore.Hal.Inspectors;
using Passless.AspNetCore.Hal.Internal;

namespace Tests
{
    public class HalMiddlewareTests
    {
        private Mock<RequestDelegate> Next => new Mock<RequestDelegate>();
        private Mock<IUrlHelperFactory> UrlHelperFactory => new Mock<IUrlHelperFactory>();
        private ILogger<HalMiddleware> Logger => Constants.LoggerFactory.CreateLogger<HalMiddleware>();
        private HttpContext HttpContext => new DefaultHttpContext();
        private Mock<IOptions<HalOptions>> HalOptions
        {
            get
            {
                var mock = new Mock<IOptions<HalOptions>>();
                mock.SetupGet(m => m.Value)
                    .Returns(new HalOptions());
                return mock;
            }
        }

        [Test]
        public async Task SetsHalFeature()
        {
            var next = Next;
            next.Setup(r => r(It.IsAny<HttpContext>()))
                .Returns(Task.CompletedTask)
                .Callback<HttpContext>(context =>
                {
                    var halFeature = context.Features.Get<HalFeature>();
                    Assert.NotNull(halFeature);
                })
                .Verifiable();

            var middleware = new HalMiddleware(
                next.Object,
                Logger,
                UrlHelperFactory.Object,
                HalOptions.Object,
                Constants.LoggerFactory);

            await middleware.Invoke(HttpContext);
            next.Verify();
        }

        [Test]
        public async Task NoHalFormattingContext_DoesNothing()
        {
            var next = Next;
            next.Setup(r => r(It.IsAny<HttpContext>()))
                .Returns(Task.CompletedTask);

            var middleware = new HalMiddleware(
                next.Object,
                Logger,
                UrlHelperFactory.Object,
                HalOptions.Object);

            var httpContext = HttpContext;
            var response = httpContext.Response;

            await middleware.Invoke(httpContext);

            Assert.IsFalse(response.HasStarted);
        }

        [Test]
        public async Task ResponseHasStarted_DoesNothing()
        {
            var next = Next;
            next.Setup(r => r(It.IsAny<HttpContext>()))
                .Returns(Task.CompletedTask);

            var middleware = new HalMiddleware(
                next.Object,
                Logger,
                UrlHelperFactory.Object,
                HalOptions.Object,
                Constants.LoggerFactory);

            // If someone tries to write to the stream, this will throw.
            var responseBody = new Mock<Stream>(MockBehavior.Strict);
            responseBody.SetupGet(s => s.Length).Returns(2);
            var httpContext = HttpContext;
            //var bytes = Encoding.UTF8.GetBytes("{}");
            //httpContext.Response.Body.Write(bytes, 0, bytes.Length);
            httpContext.Response.Body = responseBody.Object;
            await middleware.Invoke(httpContext);
        }

        // TODO: Test resourcefactory is invoked.
        // TODO: Test resourcefactory is invoked for subresources as well.
        // TODO: Test only root resourceinspectors are invoked for the root resource.
        // TODO: Test only embed resourceinspectors are invoked for the embedded resources.

        [Test]
        public async Task ResourceInspectors_AreInvokedInSequence()
        {
            var next = Next;
            next.Setup(r => r(It.IsAny<HttpContext>()))
                .Callback<HttpContext>(c =>
                {
                    var halFeature = c.Features.Get<HalFeature>();
                    halFeature.FormattingContext = new HalFormattingContext(
                        new ActionContext(),
                        new ObjectResult(new object()),
                        Mock.Of<IActionResultExecutor<ObjectResult>>());
                })
                .Returns(Task.CompletedTask);

            int order = 0;
            var syncFactory = new Mock<IHalResourceInspector>();
            syncFactory.Setup(i => i.OnInspectingResource(It.IsAny<HalResourceInspectingContext>()))
                .Callback(() =>
                {
                    Assert.AreEqual(0, order++);
                })
                .Verifiable();

            syncFactory.Setup(i => i.OnInspectedResource(It.IsAny<HalResourceInspectedContext>()))
                .Callback(() =>
                {
                    Assert.AreEqual(2, order++);
                })
                .Verifiable();

            var asyncFactory = new Mock<IAsyncHalResourceInspector>();
            asyncFactory.Setup(i => i.OnResourceInspectionAsync(It.IsAny<HalResourceInspectingContext>(), It.IsAny<HalResourceInspectionDelegate>()))
                .Callback(() =>
                {
                    Assert.AreEqual(1, order++);
                })
                .Returns<HalResourceInspectingContext, HalResourceInspectionDelegate>((context, n) => Task.FromResult(new HalResourceInspectedContext(context)))
                .Verifiable();

            var options = new HalOptions
            {
                ResourceInspectors = new List<IHalResourceInspectorMetadata>
                {
                     syncFactory.Object,
                     asyncFactory.Object
                }
            };

            var iOptions = HalOptions;
            iOptions.SetupGet(o => o.Value).Returns(options);
            var middleware = new HalMiddleware(
                next.Object,
                Logger,
                UrlHelperFactory.Object,
                iOptions.Object,
                Constants.LoggerFactory);

            await middleware.Invoke(HttpContext);
            syncFactory.Verify();
            asyncFactory.Verify();
        }
    }
}
