using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Passless.AspNetCore.Hal;
using Passless.AspNetCore.Hal.Extensions;
using Passless.AspNetCore.Hal.Formatters;
using Passless.AspNetCore.Hal.Models;

namespace Tests
{
    public class HalJsonOutputFormatterTests
    {
        [Test]
        public void CanConvertAnyType(
            [Values(
                typeof(object), 
                typeof(IResource), 
                typeof(Resource), 
                typeof(Resource<object>), 
                typeof(List<object>))]Type type)
        {
            var formatter = new HalJsonOutputFormatter(
                new JsonSerializerSettings(),
                ArrayPool<char>.Shared,
                new HalOptions());

            var writeContext = new OutputFormatterWriteContext(
                new DefaultHttpContext(),
                (s, e) => null,
                type,
                null);

            bool canWrite = formatter.CanWriteResult(writeContext);
            Assert.IsTrue(canWrite);
        }

        [Test]
        public async Task UsesResourceJsonConverter()
        {
            var formatter = new HalJsonOutputFormatter(
                new JsonSerializerSettings(),
                ArrayPool<char>.Shared,
                new HalOptions());

            using (var body = new MemoryStream())
            {
                var resource = new Resource<TestResource>
                {
                    Data = new TestResource
                    {
                        Content = "HAL-lo there"
                    }
                };

                var serviceProvider = new Mock<IServiceProvider>();
                serviceProvider.Setup(p => p.GetService(typeof(ILogger<HalJsonOutputFormatter>)))
                    .Returns(new NullLogger<HalJsonOutputFormatter>());

                var response = new Mock<HttpResponse>();
                response.SetupGet(r => r.Body).Returns(body);
                var httpContext = new Mock<HttpContext>();
                httpContext.SetupGet(c => c.Response).Returns(response.Object);
                httpContext.SetupGet(c => c.RequestServices).Returns(serviceProvider.Object);

                var writeContext = new OutputFormatterWriteContext(
                    httpContext.Object,
                    (s, e) => new StreamWriter(s, e, 4096, true),
                    resource.GetType(),
                    resource);

                await formatter.WriteResponseBodyAsync(writeContext, Encoding.UTF8);

                string expected = "{\"Content\":\"HAL-lo there\"}";

                body.Seek(0, SeekOrigin.Begin);
                string actual = null;
                using (var reader = new StreamReader(body))
                {
                    actual = reader.ReadToEnd();
                }

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
