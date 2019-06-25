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
        public void Controller_GetSet()
        {
            var att = new HalEmbedActionAttribute("rel", "action");
            att.Controller = "Controller";
            
            Assert.AreEqual(att.Controller, "Controller");
        }

        [Test]
        public void IncludeLink_DefaultFalse()
        {
            var att = new HalEmbedActionAttribute("rel", "action");

            Assert.AreEqual(false, att.IncludeLink);
        }

        [Test]
        public void IncludeLink_GetSet()
        {
            var att = new HalEmbedActionAttribute("rel", "action");
            att.IncludeLink = true;

            Assert.AreEqual(true, att.IncludeLink);
        }

        [Test]
        public void IsSingular_DefaultFalse()
        {
            var att = new HalEmbedActionAttribute("rel", "action");

            Assert.AreEqual(false, att.IsSingular);
        }

        [Test]
        public void IsSingular_GetSet()
        {
            var att = new HalEmbedActionAttribute("rel", "action");
            att.IsSingular = true;

            Assert.AreEqual(true, att.IsSingular);
        }

        [Test]
        public void Parameter_DefaultNull()
        {
            var att = new HalEmbedActionAttribute("rel", "action");

            Assert.IsNull(att.Parameter);
        }

        [Test]
        public void Parameter_GetSet()
        {
            var att = new HalEmbedActionAttribute("rel", "action");
            att.Parameter = "blah";

            Assert.AreEqual("blah", att.Parameter);
        }
    }
}
