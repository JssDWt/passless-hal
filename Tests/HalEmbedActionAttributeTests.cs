using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using Passless.AspNetCore.Hal.Attributes;

namespace Tests
{
    public class HalEmbedActionAttributeTests
    {
        [Test]
        public void Ctor_ThrowsIfNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HalEmbedActionAttribute(null, null));
            Assert.Throws<ArgumentNullException>(() => new HalEmbedActionAttribute("", null));
            Assert.Throws<ArgumentNullException>(() => new HalEmbedActionAttribute(null, ""));
        }

        [Test]
        public void Ctor_Sets_Action()
        {
            var att = new HalEmbedActionAttribute("rel", "action");
            Assert.AreEqual("action", att.Action);
        }

        [Test]
        public void Ctor_Sets_Rel()
        {
            var att = new HalEmbedActionAttribute("rel", "action");
            Assert.AreEqual("rel", att.Rel);
        }

        [Test]
        public void Controller_CanGetSet()
        {
            var att = new HalEmbedActionAttribute("rel", "action");
            att.Controller = "Controller";

            Assert.AreEqual(att.Controller, "Controller");
        }

        [Test]
        public void GetEmbedUri_ThrowsIfNull()
        {
            var att = new HalEmbedActionAttribute("rel", "action");
            Assert.Throws<ArgumentNullException>(() => att.GetEmbedUri(null));
        }
        
        [Test]
        public void GetEmbedUri_InvokesUrlHelper()
        {
            var att = new HalEmbedActionAttribute("rel", "action");
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                .Returns("")
                .Verifiable();

            att.GetEmbedUri(urlHelper.Object);
            urlHelper.Verify();
        }

        [Test]
        public void GetEmbedUri_WithController_InvokesUrlHelper()
        {
            var att = new HalEmbedActionAttribute("rel", "action")
            {
                Controller = "controller"
            };

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
                .Returns("")
                .Verifiable();

            att.GetEmbedUri(urlHelper.Object);
            urlHelper.Verify();
        }
    }
}
