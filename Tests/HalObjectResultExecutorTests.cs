using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Passless.Hal;
using Passless.Hal.Formatters;
using Passless.Hal.Internal;

namespace Tests
{
    public class HalObjectResultExecutorTests
    {
        private ILogger<HalObjectResultExecutor> Logger 
            => Constants.LoggerFactory.CreateLogger<HalObjectResultExecutor>();

        private Mock<IActionResultExecutor<ObjectResult>> InnerExecutor 
            => new Mock<IActionResultExecutor<ObjectResult>>();

        private Mock<IHttpResponseStreamWriterFactory> WriterFactory
            => new Mock<IHttpResponseStreamWriterFactory>();

        private Mock<OutputFormatterSelector> FormatterSelector
            => new Mock<OutputFormatterSelector>();

        private HttpContext HttpContext => new DefaultHttpContext();
        private HalHttpContext HalContext => new HalHttpContext(
            HttpContext,
            new HttpRequestFeature());

        private ObjectResult ObjectResult => new ObjectResult(new object());

        [Test]
        public async Task HalHttpContext_SetsResourceTest()
        {
            var executor = new HalObjectResultExecutor(
                InnerExecutor.Object,
                WriterFactory.Object,
                FormatterSelector.Object,
                Logger);

            var httpContext = HalContext;
            var actionContext = new ActionContext();
            actionContext.HttpContext = httpContext;
            var resultObject = new object();
            var result = new ObjectResult(resultObject);

            await executor.ExecuteAsync(actionContext, result);

            var response = httpContext.Response as HalHttpResponse;
            Assert.AreSame(resultObject, response.Resource);
        }

        [Test]
        public async Task HalMiddlewareNotRegistered_ExecutesInnerTest()
        {
            var inner = InnerExecutor;
            inner.Setup(i => i.ExecuteAsync(It.IsAny<ActionContext>(), It.IsAny<ObjectResult>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var executor = new HalObjectResultExecutor(
                inner.Object,
                WriterFactory.Object,
                FormatterSelector.Object,
                Logger);

            var actionContext = new ActionContext
            {
                HttpContext = HttpContext
            };

            await executor.ExecuteAsync(actionContext, ObjectResult);
            inner.Verify();
        }

        [Test]
        public async Task SelectsFormatterBasedOnObjectResultTest()
        {
            var inner = InnerExecutor;
            inner.Setup(i => i.ExecuteAsync(It.IsAny<ActionContext>(), It.IsAny<ObjectResult>()))
                .Returns(Task.CompletedTask);

            var formatters = new FormatterCollection<IOutputFormatter>();
            var mediaTypes = new MediaTypeCollection();
            var result = ObjectResult;
            result.ContentTypes = mediaTypes;
            result.Formatters = formatters;

            var httpContext = HttpContext;
            httpContext.Items.Add("HalMiddlewareRegistered", true);

            var formatterSelector = FormatterSelector;
            formatterSelector.Setup(f => f.SelectFormatter(
                It.IsAny<OutputFormatterCanWriteContext>(),
                It.IsAny<IList<IOutputFormatter>>(),
                It.IsAny<MediaTypeCollection>()))
                .Callback<
                    OutputFormatterCanWriteContext, 
                    IList<IOutputFormatter>, 
                    MediaTypeCollection>((c, f, m) =>
                {
                    Assert.AreSame(formatters, f);
                    Assert.AreSame(m, mediaTypes);
                })
                .Returns(default(IOutputFormatter))
                .Verifiable();

            var executor = new HalObjectResultExecutor(
                inner.Object,
                WriterFactory.Object,
                formatterSelector.Object,
                Logger);

            var actionContext = new ActionContext
            {
                HttpContext = httpContext
            };

            await executor.ExecuteAsync(actionContext, result);
            formatterSelector.Verify();
        }

        [Test]
        public async Task IHalFormatterSelected_DoesNotSerializeTest()
        {
            var halFormatter = new Mock<IHalFormatter>();
            var outputFormatter = halFormatter.As<IOutputFormatter>();
            var formatterSelector = FormatterSelector;
            formatterSelector.Setup(f => f.SelectFormatter(
                It.IsAny<OutputFormatterCanWriteContext>(),
                It.IsAny<IList<IOutputFormatter>>(),
                It.IsAny<MediaTypeCollection>()))
                .Returns(outputFormatter.Object);

            var inner = InnerExecutor.Object;
            var executor = new HalObjectResultExecutor(
                inner,
                WriterFactory.Object,
                formatterSelector.Object,
                Logger);

            var httpContext = HttpContext;
            httpContext.Items.Add("HalMiddlewareRegistered", true);
            var actionContext = new ActionContext
            {
                HttpContext = httpContext
            };

            var result = ObjectResult;
            await executor.ExecuteAsync(actionContext, result);

            var output = (HalFormattingContext)httpContext.Items["HalFormattingContext"];
            Assert.AreSame(actionContext, output.Context);
            Assert.AreSame(result, output.Result);
            Assert.AreSame(inner, output.Executor);
        }

        [Test]
        public async Task IHalFormatterNotSelected_DoesSerializeTest()
        {
            var outputFormatter = Mock.Of<IOutputFormatter>();
            var formatterSelector = FormatterSelector;
            formatterSelector.Setup(f => f.SelectFormatter(
                It.IsAny<OutputFormatterCanWriteContext>(),
                It.IsAny<IList<IOutputFormatter>>(),
                It.IsAny<MediaTypeCollection>()))
                .Returns(outputFormatter);

            var inner = InnerExecutor;
            inner.Setup(i => i.ExecuteAsync(It.IsAny<ActionContext>(), It.IsAny<ObjectResult>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var executor = new HalObjectResultExecutor(
                inner.Object,
                WriterFactory.Object,
                formatterSelector.Object,
                Logger);

            var httpContext = HttpContext;
            httpContext.Items.Add("HalMiddlewareRegistered", true);
            var actionContext = new ActionContext
            {
                HttpContext = httpContext
            };

            await executor.ExecuteAsync(actionContext, ObjectResult);
            inner.Verify();
        }
    }
}
