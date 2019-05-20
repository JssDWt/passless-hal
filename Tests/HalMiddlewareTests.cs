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
using Passless.Hal;
using Passless.Hal.Extensions;
using Passless.Hal.Internal;

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
        public async Task SetsHalMiddlewareRegistered()
        {
            var next = Next;
            next.Setup(r => r(It.IsAny<HttpContext>()))
                .Returns(Task.CompletedTask)
                .Callback<HttpContext>(context =>
                {
                    var registered = context.Items["HalMiddlewareRegistered"];
                    Assert.AreEqual(true, registered);
                })
                .Verifiable();

            var middleware = new HalMiddleware(
                next.Object,
                Logger,
                UrlHelperFactory.Object,
                HalOptions.Object);

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
                HalOptions.Object);

            // If someone tries to write to the stream, this will throw.
            var responseBody = new Mock<Stream>(MockBehavior.Strict);
            responseBody.SetupGet(s => s.Length).Returns(2);
            var httpContext = HttpContext;
            //var bytes = Encoding.UTF8.GetBytes("{}");
            //httpContext.Response.Body.Write(bytes, 0, bytes.Length);
            httpContext.Response.Body = responseBody.Object;
            await middleware.Invoke(httpContext);
        }

        [Test]
        public async Task ResourceFactories_AreInvokedInSequence()
        {
            var next = Next;
            next.Setup(r => r(It.IsAny<HttpContext>()))
                .Callback<HttpContext>(
                    c => c.Items.Add("HalFormattingContext", new HalFormattingContext
                    {
                        Context = new ActionContext(),
                        Result = new ObjectResult(new object()),
                        Executor = Mock.Of<IActionResultExecutor<ObjectResult>>()
                    }))
                .Returns(Task.CompletedTask);

            int order = 0;
            var syncFactory = new Mock<IHalResourceFactory>();
            syncFactory.Setup(f => f.CreateResource(It.IsAny<HalMiddleware>(), It.IsAny<ActionContext>(), It.IsAny<ObjectResult>()))
                .Callback(() =>
                {
                    Assert.AreEqual(0, order++);
                })
                .Verifiable();

            var asyncFactory = new Mock<IAsyncHalResourceFactory>();
            asyncFactory.Setup(f => f.CreateResourceAsync(It.IsAny<HalMiddleware>(), It.IsAny<ActionContext>(), It.IsAny<ObjectResult>()))
                .Callback(() =>
                {
                    Assert.AreEqual(1, order++);
                })
                .Returns(Task.CompletedTask)
                .Verifiable();

            var options = new HalOptions
            {
                ResourceFactories = new List<IHalResourceFactoryMetadata>
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
                iOptions.Object);

            await middleware.Invoke(HttpContext);
            syncFactory.Verify();
            asyncFactory.Verify();
        }
    }
}
