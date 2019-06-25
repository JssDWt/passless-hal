using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using NUnit.Framework;
using Passless.AspNetCore.Hal.Attributes;

namespace Tests
{
    public class HalLinkActionAttributeTests
    {
        [Test]
        public void Ctor_ThrowsIfNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HalLinkActionAttribute(null, null));
            Assert.Throws<ArgumentNullException>(() => new HalLinkActionAttribute("", null));
            Assert.Throws<ArgumentNullException>(() => new HalLinkActionAttribute(null, ""));
        }

        [Test]
        public void Ctor_Sets_Action()
        {
            var att = new HalLinkActionAttribute("rel", "action");
            Assert.AreEqual("action", att.Action);
        }

        [Test]
        public void Ctor_Sets_Rel()
        {
            var att = new HalLinkActionAttribute("rel", "action");
            Assert.AreEqual("rel", att.Rel);
        }

        [Test]
        public void Controller_GetSet()
        {
            var att = new HalLinkActionAttribute("rel", "action");
            att.Controller = "Controller";

            Assert.AreEqual(att.Controller, "Controller");
        }

        [Test]
        public void IsSingular_DefaultFalse()
        {
            var att = new HalLinkActionAttribute("rel", "action");

            Assert.AreEqual(false, att.IsSingular);
        }

        [Test]
        public void IsSingular_GetSet()
        {
            var att = new HalLinkActionAttribute("rel", "action");
            att.IsSingular = true;

            Assert.AreEqual(true, att.IsSingular);
        }

        [Test]
        public void Parameter_DefaultNull()
        {
            var att = new HalLinkActionAttribute("rel", "action");

            Assert.IsNull(att.Parameter);
        }

        [Test]
        public void Parameter_GetSet()
        {
            var att = new HalLinkActionAttribute("rel", "action");
            att.Parameter = "blah";

            Assert.AreEqual("blah", att.Parameter);
        }
    }
}
