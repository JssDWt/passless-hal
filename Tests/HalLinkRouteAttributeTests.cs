using System;
using NUnit.Framework;
using Passless.AspNetCore.Hal.Attributes;

namespace Tests
{
    public class HalLinkRouteAttributeTests
    {
        [Test]
        public void Ctor_ThrowsIfNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HalLinkRouteAttribute(null, null));
            Assert.Throws<ArgumentNullException>(() => new HalLinkRouteAttribute("", null));
            Assert.Throws<ArgumentNullException>(() => new HalLinkRouteAttribute(null, ""));
        }

        [Test]
        public void Ctor_Sets_Action()
        {
            var att = new HalLinkRouteAttribute("rel", "routeName");
            Assert.AreEqual("routeName", att.RouteName);
        }

        [Test]
        public void Ctor_Sets_Rel()
        {
            var att = new HalLinkRouteAttribute("rel", "routeName");
            Assert.AreEqual("rel", att.Rel);
        }

        [Test]
        public void IsSingular_DefaultFalse()
        {
            var att = new HalLinkRouteAttribute("rel", "routeName");

            Assert.AreEqual(false, att.IsSingular);
        }

        [Test]
        public void IsSingular_GetSet()
        {
            var att = new HalLinkRouteAttribute("rel", "routeName");
            att.IsSingular = true;

            Assert.AreEqual(true, att.IsSingular);
        }

        [Test]
        public void Parameter_DefaultNull()
        {
            var att = new HalLinkRouteAttribute("rel", "routeName");

            Assert.IsNull(att.Parameter);
        }

        [Test]
        public void Parameter_GetSet()
        {
            var att = new HalLinkRouteAttribute("rel", "routeName");
            att.Parameter = "blah";

            Assert.AreEqual("blah", att.Parameter);
        }
    }
}
