using System;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Passless.AspNetCore.Hal.Extensions;
using Passless.AspNetCore.Hal.Factories;

namespace Tests
{
    public class HalSetupTests
    {
        private Mock<IServiceProvider> ServiceProvider => new Mock<IServiceProvider>();
        private Mock<IHalResourceFactoryMetadata> ResourceFactory => new Mock<IHalResourceFactoryMetadata>();

        [Test]
        public void Ctor_ThrowsIfNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HalSetup(null, null));
            Assert.Throws<ArgumentNullException>(() => new HalSetup(ServiceProvider.Object, null));
            Assert.Throws<ArgumentNullException>(() => new HalSetup(null, ResourceFactory.Object));
        }

        [Test]
        public void Configure_OptionsNullThrows()
        {
            IConfigureOptions<HalOptions> options = new HalSetup(ServiceProvider.Object, ResourceFactory.Object);
            Assert.Throws<ArgumentNullException>(() => options.Configure(null));
        }

        [Test]
        public void Configure_UseDefaultResourceInspectorsFalse_DoesNotAddResourceInspectors()
        {
            var resourceFactory = ResourceFactory.Object;
            IConfigureOptions<HalOptions> setup = new HalSetup(ServiceProvider.Object, resourceFactory);
            var options = new HalOptions
            {
                UseDefaultResourceInspectors = false
            };

            setup.Configure(options);
            Assert.AreEqual(0, options.ResourceInspectors.Count);
        }

        // TODO: Test adding default resourceinspectors.
    }
}
