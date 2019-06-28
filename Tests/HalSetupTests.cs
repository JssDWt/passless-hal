using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Passless.AspNetCore.Hal.Extensions;
using Passless.AspNetCore.Hal.Factories;
using Passless.AspNetCore.Hal.Inspectors;
using Passless.AspNetCore.Hal.Internal;

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

        [Test]
        public void Configure_UseDefaultResourceInspectors_AddsAttributeEmbedInspector()
        {
            var services = new ServiceCollection();
            services.AddMvcCore();
            services.AddLogging();
            services.AddSingleton(Mock.Of<LinkService>());
            var provider = services.BuildServiceProvider();
            IConfigureOptions<HalOptions> setup = new HalSetup(provider, ResourceFactory.Object);
            var options = new HalOptions
            {
                UseDefaultResourceInspectors = true
            };

            setup.Configure(options);
            Assert.That(options.ResourceInspectors, Has.One.InstanceOf<AttributeEmbedInspector>());
        }

        [Test]
        public void Configure_UseDefaultResourceInspectors_AddsAttributeLinkInspector()
        {
            var services = new ServiceCollection();
            services.AddMvcCore();
            services.AddLogging();
            services.AddSingleton(Mock.Of<LinkService>());
            var provider = services.BuildServiceProvider();
            IConfigureOptions<HalOptions> setup = new HalSetup(provider, ResourceFactory.Object);
            var options = new HalOptions
            {
                UseDefaultResourceInspectors = true
            };

            setup.Configure(options);
            Assert.That(options.ResourceInspectors, Has.One.InstanceOf<AttributeLinkInspector>());
        }

        [Test]
        public void Configure_UseDefaultResourceInspectors_AddsResourceValidationInspector()
        {
            var services = new ServiceCollection();
            services.AddMvcCore();
            services.AddLogging();
            services.AddSingleton(Mock.Of<LinkService>());
            var provider = services.BuildServiceProvider();
            IConfigureOptions<HalOptions> setup = new HalSetup(provider, ResourceFactory.Object);
            var options = new HalOptions
            {
                UseDefaultResourceInspectors = true
            };

            setup.Configure(options);
            Assert.That(options.ResourceInspectors, Has.One.InstanceOf<ResourceValidationInspector>());
        }

        [Test]
        public void Configure_UseDefaultResourceInspectors_AddsLinkPermissionInspector()
        {
            var services = new ServiceCollection();
            services.AddMvcCore();
            services.AddLogging();
            services.AddSingleton(Mock.Of<LinkService>());
            var provider = services.BuildServiceProvider();
            IConfigureOptions<HalOptions> setup = new HalSetup(provider, ResourceFactory.Object);
            var options = new HalOptions
            {
                UseDefaultResourceInspectors = true
            };

            setup.Configure(options);
            Assert.That(options.ResourceInspectors, Has.One.InstanceOf<LinkPermissionInspector>());
        }
    }
}
