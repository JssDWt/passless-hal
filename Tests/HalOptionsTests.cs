using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Passless.AspNetCore.Hal.Extensions;
using Passless.AspNetCore.Hal.Factories;
using Passless.AspNetCore.Hal.Inspectors;

namespace Tests
{
    public class HalOptionsTests
    {
        [Test]
        public void SupportedMediaTypes_DefaultHalJson()
        {
            var options = new HalOptions();
            var mediaTypes = options.SupportedMediaTypes;
            Assert.AreEqual(1, mediaTypes.Count);
            Assert.AreEqual("application/hal+json", mediaTypes.First());
        }

        [Test]
        public void SupportedMediaTypes_ThrowsIfNull()
        {
            var options = new HalOptions();
            Assert.Throws<ArgumentNullException>(() => options.SupportedMediaTypes = null);
        }

        [Test]
        public void SupportedMediaTypes_GetSet()
        {
            var options = new HalOptions();
            var mediaTypes = new List<string>();
            options.SupportedMediaTypes = mediaTypes;
            Assert.AreSame(mediaTypes, options.SupportedMediaTypes);
        }

        [Test]
        public void ResourceFactory_ThrowsIfNull()
        {
            var options = new HalOptions();
            Assert.Throws<ArgumentNullException>(() => options.ResourceFactory = null);
        }

        [Test]
        public void ResourceFactory_GetSet()
        {
            var options = new HalOptions();
            var factory = Mock.Of<IHalResourceFactoryMetadata>();
            options.ResourceFactory = factory;
            Assert.AreSame(factory, options.ResourceFactory);
        }

        [Test]
        public void ResourceInspectors_DefaultInitialized()
        {
            var options = new HalOptions();
            Assert.NotNull(options.ResourceInspectors);
        }

        [Test]
        public void ResourceInspectors_GetSet()
        {
            var options = new HalOptions();
            var inspectors = Mock.Of<IList<IHalResourceInspectorMetadata>>();
            options.ResourceInspectors = inspectors;
            Assert.AreSame(inspectors, options.ResourceInspectors);
        }

        [Test]
        public void UseDefaultResourceFactory_DefaultTrue()
        {
            var options = new HalOptions();
            Assert.IsTrue(options.UseDefaultResourceFactory);
        }

        [Test]
        public void UseDefaultResourceFactory_GetSet()
        {
            var options = new HalOptions();
            options.UseDefaultResourceFactory = false;
            Assert.IsFalse(options.UseDefaultResourceFactory);
        }

        [Test]
        public void UseDefaultResourceInspectors_DefaultTrue()
        {
            var options = new HalOptions();
            Assert.IsTrue(options.UseDefaultResourceInspectors);
        }

        [Test]
        public void UseDefaultResourceInspectors_GetSet()
        {
            var options = new HalOptions();
            options.UseDefaultResourceInspectors = false;
            Assert.IsFalse(options.UseDefaultResourceInspectors);
        }
    }
}
