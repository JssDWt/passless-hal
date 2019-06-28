using System;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Passless.AspNetCore.Hal.Inspectors;
using Passless.AspNetCore.Hal.Internal;

namespace Tests
{
    public class AttributeEmbedInspectorTests
    {
        private Mock<IUrlHelperFactory> UrlHelper => new Mock<IUrlHelperFactory>();
        private ILogger<AttributeEmbedInspector> Logger => NullLogger<AttributeEmbedInspector>.Instance;
        private Mock<LinkService> LinkService => new Mock<LinkService>();

        [Test]
        public void Ctor_ThrowsIfNull()
        {
            Assert.Throws<ArgumentNullException>(() => new AttributeEmbedInspector(null, Logger, LinkService.Object));
            Assert.Throws<ArgumentNullException>(() => new AttributeEmbedInspector(UrlHelper.Object, null, LinkService.Object));
            Assert.Throws<ArgumentNullException>(() => new AttributeEmbedInspector(UrlHelper.Object, Logger, null));
        }

        [Test]
        public void UseOnEmbeddedResources_IsFalse()
        {
            // Arrange
            var inspector = CreateDefaultInspector();

            // Act
            bool useOnEmbedded = inspector.UseOnEmbeddedResources;

            // Assert
            Assert.IsFalse(useOnEmbedded);
        }

        [Test]
        public void UseOnRootResource_IsTrue()
        {
            // Arrange
            var inspector = CreateDefaultInspector();

            // Act
            bool useOnRoot = inspector.UseOnRootResource;

            // Assert
            Assert.IsTrue(useOnRoot);
        }

        private AttributeEmbedInspector CreateDefaultInspector()
            => new AttributeEmbedInspector(UrlHelper.Object, Logger, LinkService.Object);
    }
}
