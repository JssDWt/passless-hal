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
        private Mock<ResourcePipelineInvoker> Pipeline => new Mock<ResourcePipelineInvoker>();
        private Mock<ResourcePipelineInvokerFactory> PipelineFactory => new Mock<ResourcePipelineInvokerFactory>();
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
        public void Ctor_ThrowsIfNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HalMiddleware(null, Logger, HalOptions.Object, PipelineFactory.Object));
            Assert.Throws<ArgumentNullException>(() => new HalMiddleware(Next.Object, null, HalOptions.Object, PipelineFactory.Object));
            Assert.Throws<ArgumentNullException>(() => new HalMiddleware(Next.Object, Logger, HalOptions.Object, null));
        }

        [Test]
        public void Ctor_GivesMissingAddHalIndication()
        {
            var ex = Assert.Throws<ArgumentException>(() => new HalMiddleware(Next.Object, Logger, null, PipelineFactory.Object));
            StringAssert.Contains("AddHal", ex.Message);
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
                HalOptions.Object,
                PipelineFactory.Object);

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
                HalOptions.Object,
                PipelineFactory.Object);

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
                HalOptions.Object,
                PipelineFactory.Object);

            // If someone tries to write to the stream, this will throw.
            var responseBody = new Mock<Stream>(MockBehavior.Strict);
            responseBody.SetupGet(s => s.Length).Returns(2);
            var httpContext = HttpContext;
            httpContext.Response.Body = responseBody.Object;

            await middleware.Invoke(httpContext);
            next.Verify();
        }

        [Test]
        public async Task ResourcePipeline_CreatedAndInvoked()
        {
            var next = Next;
            next.Setup(r => r(It.IsAny<HttpContext>()))
                .Returns(Task.CompletedTask)
                .Callback<HttpContext>(context =>
                {
                    var halFeature = context.Features.Get<HalFeature>();
                    halFeature.FormattingContext = new HalFormattingContext(
                        Mock.Of<ActionContext>(),
                        new ObjectResult(new object()),
                        Mock.Of<IActionResultExecutor<ObjectResult>>());
                });

            var pipeline = Pipeline;
            pipeline.Setup(p => p.InvokeAsync(It.IsAny<HalFormattingContext>()))
                .Returns(() => Task.FromResult(new ObjectResult(new object())))
                .Verifiable();
            var pipelineFactory = PipelineFactory;
            pipelineFactory.Setup(p => p.Create(It.IsAny<MvcPipeline>()))
                .Returns(pipeline.Object)
                .Verifiable();
                
            var middleware = new HalMiddleware(
                next.Object,
                Logger,
                HalOptions.Object,
                pipelineFactory.Object);

            await middleware.Invoke(HttpContext);

            pipelineFactory.Verify();
            pipeline.Verify();
        }

        [Test]
        public async Task ResourcePipeline_ResultPassedToExecutor()
        {
            var actionContext = Mock.Of<ActionContext>();
            var objectResult = new ObjectResult(new object());
            var executorMock = new Mock<IActionResultExecutor<ObjectResult>>();
            executorMock.Setup(e => e.ExecuteAsync(actionContext, objectResult))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var formattingContext = new HalFormattingContext(
                actionContext,
                new ObjectResult(new object()),
                executorMock.Object);

            var next = Next;
            next.Setup(r => r(It.IsAny<HttpContext>()))
                .Returns(Task.CompletedTask)
                .Callback<HttpContext>((context) =>
                {
                    var halFeature = context.Features.Get<HalFeature>();
                    halFeature.FormattingContext = formattingContext;
                });
            
            var pipeline = Pipeline;
            pipeline.Setup(p => p.InvokeAsync(It.IsAny<HalFormattingContext>()))
                .Returns(() => Task.FromResult(objectResult))
                .Verifiable();
            var pipelineFactory = PipelineFactory;
            pipelineFactory.Setup(p => p.Create(It.IsAny<MvcPipeline>()))
                .Returns(pipeline.Object)
                .Verifiable();

            var middleware = new HalMiddleware(
                next.Object,
                Logger,
                HalOptions.Object,
                pipelineFactory.Object);

            await middleware.Invoke(HttpContext);

            executorMock.Verify();
        }
    }
}
