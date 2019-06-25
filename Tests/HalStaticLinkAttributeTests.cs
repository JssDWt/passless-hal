using System;
using NUnit.Framework;
using Passless.AspNetCore.Hal.Attributes;

namespace Tests
{
    public class HalStaticLinkAttributeTests
    {
        [Test]
        public void Ctor_ThrowsIfNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HalStaticLinkAttribute(null, null));
            Assert.Throws<ArgumentNullException>(() => new HalStaticLinkAttribute("", null));
            Assert.Throws<ArgumentNullException>(() => new HalStaticLinkAttribute(null, ""));
        }

        [Test]
        public void Ctor_Sets_Url()
        {
            var att = new HalStaticLinkAttribute("rel", "/url");
            Assert.AreEqual("/url", att.Url);
        }

        [Test]
        public void Ctor_Sets_Rel()
        {
            var att = new HalStaticLinkAttribute("rel", "/url");
            Assert.AreEqual("rel", att.Rel);
        }

        [Test]
        public void IsSingular_DefaultFalse()
        {
            var att = new HalStaticLinkAttribute("rel", "/url");

            Assert.AreEqual(false, att.IsSingular);
        }

        [Test]
        public void IsSingular_GetSet()
        {
            var att = new HalStaticLinkAttribute("rel", "/url");
            att.IsSingular = true;

            Assert.AreEqual(true, att.IsSingular);
        }
    }
}
