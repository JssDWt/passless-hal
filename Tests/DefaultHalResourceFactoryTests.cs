using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Passless.AspNetCore.Hal;
using Passless.AspNetCore.Hal.Factories;
using Passless.AspNetCore.Hal.Models;

namespace Tests
{
    public class DefaultHalResourceFactoryTests
    {
        [Test]
        public void CreateResource_ThrowsIfNull()
        {
            var factory = new DefaultHalResourceFactory();
            Assert.Throws<ArgumentNullException>(() => factory.CreateResource(null));
        }

        [Test]
        public void CreateResource_IResourceUnchanged()
        {
            var factory = new DefaultHalResourceFactory();
            var resource = Mock.Of<IResource>();
            var context = new ResourceFactoryContext
            {
                Resource = resource
            };

            var result = factory.CreateResource(context);
            Assert.AreSame(resource, result);
        }

        [Test]
        public void CreateResource_NullResourceIsFine()
        {
            var factory = new DefaultHalResourceFactory();
            object resource = null;
            var context = new ResourceFactoryContext
            {
                Resource = resource
            };

            var result = factory.CreateResource(context);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Resource<object>>(result);
            var casted = (Resource<object>)result;
            Assert.IsNull(casted.Data);
        }

        [Test]
        public void CreateResource_EnumerableCreatesCollection()
        {
            var factory = new DefaultHalResourceFactory();
            object resource = new List<string>();
            var context = new ResourceFactoryContext
            {
                Resource = resource
            };

            var result = factory.CreateResource(context);
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ResourceCollection<object>>(result);
        }

        [Test]
        public void CreateResource_EnumerableIncludedInCollection()
        {
            var factory = new DefaultHalResourceFactory();
            object resource = new List<object> { new object(), new object() };
            var context = new ResourceFactoryContext
            {
                Resource = resource
            };

            var result = factory.CreateResource(context);
            var casted = (IResourceCollection)result;
            Assert.AreEqual(2, casted.Collection.Count);
        }

        [Test]
        public void CreateResource_NoResource_Wrapped()
        {
            var factory = new DefaultHalResourceFactory();
            object resource = new object();
            var context = new ResourceFactoryContext
            {
                Resource = resource
            };

            var result = factory.CreateResource(context);
            Assert.IsInstanceOf<Resource<object>>(result);
            var casted = (Resource<object>)result;
            Assert.AreSame(resource, casted.Data);
        }
    }
}
