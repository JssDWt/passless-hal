using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Passless.AspNetCore.Hal.Extensions;
using Passless.AspNetCore.Hal.Internal;

namespace Tests
{
    public class MvcBuilderExtensionsTests
    {
        // TODO: Test no scoped service is referred to by a singleton
        [Test]
        public void AddHal_AddsResourceInspectorSelector()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            collection.AddMvcCore().AddHal();

            // Assert
            Assert.That(collection, Has.One.With.Property(nameof(ServiceDescriptor.ServiceType)).EqualTo(typeof(ResourceInspectorSelector)));

            Assert.NotNull(collection.SingleOrDefault(sd => sd.ServiceType == typeof(ResourceInspectorSelector)));
        }

        [Test]
        public void AddHal_AddsResourcePipelineInvokerFactory()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            collection.AddMvcCore().AddHal();

            // Assert
            Assert.NotNull(collection.SingleOrDefault(sd => sd.ServiceType == typeof(ResourcePipelineInvokerFactory)));
        }

        [Test]
        public void AddHal_AddsLinkService()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            collection.AddMvcCore().AddHal();

            // Assert
            Assert.NotNull(collection.SingleOrDefault(sd => sd.ServiceType == typeof(LinkService)));
        }

        [Test]
        public void AddHal_AddsParameterParser()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            collection.AddMvcCore().AddHal();

            // Assert
            Assert.NotNull(collection.SingleOrDefault(sd => sd.ServiceType == typeof(ParameterParser)));
        }

        [Test]
        public void AddHal_AddsValueMapper()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            collection.AddMvcCore().AddHal();

            // Assert
            Assert.NotNull(collection.SingleOrDefault(sd => sd.ServiceType == typeof(ValueMapper)));
        }

        [Test]
        public void AddHal_AddsActionUriService()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            collection.AddMvcCore().AddHal();

            // Assert
            Assert.NotNull(collection.SingleOrDefault(sd =>
                sd.ServiceType == typeof(IUriService<IActionDescriptor>)
                && sd.ImplementationType == typeof(ActionUriService)));
        }

        [Test]
        public void AddHal_AddsRouteUriService()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            collection.AddMvcCore().AddHal();

            // Assert
            Assert.NotNull(collection.SingleOrDefault(sd =>
                sd.ServiceType == typeof(IUriService<IRouteDescriptor>)
                && sd.ImplementationType == typeof(RouteUriService)));
        }

        [Test]
        public void AddHal_AddsActionContextAccessor()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            collection.AddMvcCore().AddHal();

            // Assert
            Assert.NotNull(collection.SingleOrDefault(sd =>
                sd.ServiceType == typeof(IActionContextAccessor)));
        }

        [Test]
        public void AddHal_DecoratesObjectResultExecutor()
        {
            // Arrange
            var collection = new ServiceCollection();
            collection.AddLogging();

            // Act
            collection.AddMvcCore().AddHal();

            // Rearrange
            var executorService = collection.SingleOrDefault(sd => sd.ServiceType == typeof(IActionResultExecutor<ObjectResult>));
            var factory = executorService.ImplementationFactory;
            var provider = collection.BuildServiceProvider();
            var executor = factory(provider);

            // Assert
            Assert.IsInstanceOf<HalObjectResultExecutor>(executor);
        }

        [Test]
        public void AddHal_AddsHalSetup()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            collection.AddMvcCore().AddHal();

            // Assert
            Assert.NotNull(collection.SingleOrDefault(sd =>
                sd.ServiceType == typeof(IConfigureOptions<HalOptions>)
                && sd.ImplementationType == typeof(HalSetup)));
        }

        [Test]
        public void AddHal_AddsHalMvcSetup()
        {
            // Arrange
            var collection = new ServiceCollection();

            // Act
            collection.AddMvcCore().AddHal();

            // Assert
            Assert.NotNull(collection.SingleOrDefault(sd =>
                sd.ServiceType == typeof(IConfigureOptions<MvcOptions>)
                && sd.ImplementationType == typeof(HalMvcSetup)));

            Assert.NotNull(collection.SingleOrDefault(sd =>
                sd.ServiceType == typeof(IPostConfigureOptions<MvcOptions>)
                && sd.ImplementationType == typeof(HalMvcSetup)));
        }
    }
}
