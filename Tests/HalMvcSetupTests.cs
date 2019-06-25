using System;
using System.Buffers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Passless.AspNetCore.Hal.Extensions;
using Passless.AspNetCore.Hal.Formatters;
using Passless.AspNetCore.Hal.Internal;

namespace Tests
{
    public class HalMvcSetupTests
    {
        private Mock<IOptions<HalOptions>> HalOptions
        {
            get
            {
                var mock = new Mock<IOptions<HalOptions>>();
                mock.SetupGet(m => m.Value).Returns(Mock.Of<HalOptions>());
                return mock;
            }
        }
    
        private Mock<IOptions<MvcJsonOptions>> JsonOptions
        {
            get
            {
                var mock = new Mock<IOptions<MvcJsonOptions>>();
                mock.SetupGet(m => m.Value).Returns(Mock.Of<MvcJsonOptions>());
                return mock;
            }
        }
        private Mock<ArrayPool<char>> ArrayPool => new Mock<ArrayPool<char>>();

        [Test]
        public void Ctor_ThrowsIfNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HalMvcSetup(null, null, null));
            Assert.Throws<ArgumentNullException>(() => new HalMvcSetup(HalOptions.Object, JsonOptions.Object, null));
            Assert.Throws<ArgumentNullException>(() => new HalMvcSetup(HalOptions.Object, null, ArrayPool.Object));
            Assert.Throws<ArgumentNullException>(() => new HalMvcSetup(null, JsonOptions.Object, ArrayPool.Object));
        }

        [Test]
        public void Ctor_ThrowsIfIOptionsValueNull()
        {
            Assert.Throws<ArgumentException>(() => new HalMvcSetup(Mock.Of<IOptions<HalOptions>>(), JsonOptions.Object, ArrayPool.Object));
            Assert.Throws<ArgumentException>(() => new HalMvcSetup(HalOptions.Object, Mock.Of<IOptions<MvcJsonOptions>>(), ArrayPool.Object));
        }

        [Test]
        public void Configure_InsertsFormatterAtStart()
        {
            var setup = new HalMvcSetup(HalOptions.Object, JsonOptions.Object, ArrayPool.Object);
            var mvcOptions = new MvcOptions();
            mvcOptions.OutputFormatters.Add(Mock.Of<IOutputFormatter>());
            setup.Configure(mvcOptions);
            Assert.AreEqual(2, mvcOptions.OutputFormatters.Count);
            Assert.IsInstanceOf<HalJsonOutputFormatter>(mvcOptions.OutputFormatters[0]);
        }

        [Test]
        public void PostConfigure_InsertsLinkValidationFilter()
        {
            var setup = new HalMvcSetup(HalOptions.Object, JsonOptions.Object, ArrayPool.Object);
            var mvcOptions = new MvcOptions();
            
            setup.PostConfigure("", mvcOptions);
            var filter = mvcOptions.Filters[0] as ServiceFilterAttribute;
            Assert.AreEqual(typeof(LinkValidationFilter), filter.ServiceType);
            Assert.AreEqual(0, filter.Order);
        }
    }
}
