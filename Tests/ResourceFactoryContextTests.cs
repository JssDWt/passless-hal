using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Passless.AspNetCore.Hal;
using Passless.AspNetCore.Hal.Factories;

namespace Tests
{
    public class ResourceFactoryContextTests
    {
        [Test]
        public void Ctor_HasDefaultConstructor()
        {
            // Won't compile if not true
            var context = new ResourceFactoryContext();
        }

        [Test]
        public void ActionContext_GetSet()
        {
            // Arrange
            var context = new ResourceFactoryContext();
            var stub = Mock.Of<ActionContext>();

            // Act
            context.ActionContext = stub;

            // Assert
            Assert.AreSame(stub, context.ActionContext);
        }

        [Test]
        public void Resource_GetSet()
        {
            // Arrange
            var context = new ResourceFactoryContext();
            var stub = Mock.Of<IResource>();

            // Act
            context.Resource = stub;

            // Assert
            Assert.AreSame(stub, context.Resource);
        }

        [Test]
        public void IsRootResource_DefaultFalse()
        {
            // Arrange
            var context = new ResourceFactoryContext();

            // Act
            bool isRoot = context.IsRootResource;

            // Assert
            Assert.IsFalse(isRoot);
        }

        [Test]
        public void IsRootResource_GetSet()
        {
            // Arrange
            var context = new ResourceFactoryContext();

            // Act
            context.IsRootResource = true;
            bool isRoot = context.IsRootResource;

            // Assert
            Assert.IsTrue(isRoot);
        }
    }
}
