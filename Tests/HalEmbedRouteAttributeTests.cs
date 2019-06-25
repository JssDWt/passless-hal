using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using Passless.AspNetCore.Hal.Attributes;

namespace Tests
{
    public class HalEmbedRouteAttributeTests
    {
        [Test]
        public void Ctor_ThrowsIfNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HalEmbedRouteAttribute(null, null));
            Assert.Throws<ArgumentNullException>(() => new HalEmbedRouteAttribute("", null));
            Assert.Throws<ArgumentNullException>(() => new HalEmbedRouteAttribute(null, ""));
        }

        [Test]
        public void Ctor_Sets_Action()
        {
            var att = new HalEmbedRouteAttribute("rel", "routeName");
            Assert.AreEqual("routeName", att.RouteName);
        }

        [Test]
        public void Ctor_Sets_Rel()
        {
            var att = new HalEmbedRouteAttribute("rel", "routeName");
            Assert.AreEqual("rel", att.Rel);
        }

        [Test]
        public void GetEmbedUri_ThrowsIfNull()
        {
            var att = new HalEmbedRouteAttribute("rel", "routeName");
            Assert.Throws<ArgumentNullException>(() => att.GetEmbedUri(null));
        }

        [Test]
        public void GetEmbedUri_InvokesUrlHelper()
        {
            var att = new HalEmbedRouteAttribute("rel", "routeName");
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(u => u.RouteUrl(It.IsAny<UrlRouteContext>()))
                .Returns("")
                .Verifiable();

            att.GetEmbedUri(urlHelper.Object);
            urlHelper.Verify();
        }
    }
}
