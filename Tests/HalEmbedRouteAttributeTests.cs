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
        public void IncludeLink_DefaultFalse()
        {
            var att = new HalEmbedRouteAttribute("rel", "routeName");

            Assert.AreEqual(false, att.IncludeLink);
        }

        [Test]
        public void IncludeLink_GetSet()
        {
            var att = new HalEmbedRouteAttribute("rel", "routeName");
            att.IncludeLink = true;
            
            Assert.AreEqual(true, att.IncludeLink);
        }

        [Test]
        public void IsSingular_DefaultFalse()
        {
            var att = new HalEmbedRouteAttribute("rel", "routeName");

            Assert.AreEqual(false, att.IsSingular);
        }

        [Test]
        public void IsSingular_GetSet()
        {
            var att = new HalEmbedRouteAttribute("rel", "routeName");
            att.IsSingular = true;
            
            Assert.AreEqual(true, att.IsSingular);
        }

        [Test]
        public void Parameter_DefaultNull()
        {
            var att = new HalEmbedRouteAttribute("rel", "routeName");

            Assert.IsNull(att.Parameter);
        }

        [Test]
        public void Parameter_GetSet()
        {
            var att = new HalEmbedRouteAttribute("rel", "routeName");
            att.Parameter = "blah";
            
            Assert.AreEqual("blah", att.Parameter);
        }
    }
}
