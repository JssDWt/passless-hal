using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Passless.AspNetCore.Hal;
using Passless.AspNetCore.Hal.Factories;
using Passless.AspNetCore.Hal.Inspectors;
using Passless.AspNetCore.Hal.Internal;

namespace Tests
{
    public class ResourceInspectorInvokerTests
    {
        [Test]
        public async Task InvokeAsync_ResourceInspectors_AreInvokedInSequence()
        {
            int order = 0;
            var syncInspector = new Mock<IHalResourceInspector>();
            syncInspector.Setup(i => i.OnInspectingResource(It.IsAny<HalResourceInspectingContext>()))
                .Callback(() =>
                {
                    Assert.AreEqual(0, order++);
                })
                .Verifiable();

            syncInspector.Setup(i => i.OnInspectedResource(It.IsAny<HalResourceInspectedContext>()))
                .Callback(() =>
                {
                    Assert.AreEqual(2, order++);
                })
                .Verifiable();

            var asyncInspector = new Mock<IAsyncHalResourceInspector>();
            asyncInspector.Setup(i => i.OnResourceInspectionAsync(It.IsAny<HalResourceInspectingContext>(), It.IsAny<HalResourceInspectionDelegate>()))
                .Callback(() =>
                {
                    Assert.AreEqual(1, order++);
                })
                .Returns<HalResourceInspectingContext, HalResourceInspectionDelegate>((context, n) => Task.FromResult(new HalResourceInspectedContext(context)))
                .Verifiable();

            var inspectors = new IHalResourceInspectorMetadata[] {
                syncInspector.Object,
                asyncInspector.Object
            };

            var invoker = new HalResourceInspectorInvoker(inspectors, NullLogger<HalResourceInspectorInvoker>.Instance);
            await invoker.InspectAsync(
                new HalResourceInspectingContext(
                    Mock.Of<IResource>(),
                    Mock.Of<ActionContext>(),
                    true,
                    Mock.Of<ResourceFactory>(),
                    new MvcPipeline((HttpContext context) => Task.CompletedTask),
                    Mock.Of<object>()));

            syncInspector.Verify();
            asyncInspector.Verify();
        }
    }
}
