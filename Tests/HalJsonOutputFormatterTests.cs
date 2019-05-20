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
using Newtonsoft.Json;
using NUnit.Framework;
using Passless.Hal;
using Passless.Hal.Extensions;
using Passless.Hal.Formatters;

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
                new JsonSerializerSettings
                {

                },
                ArrayPool<char>.Shared,
                new HalOptions());

            using (var body = new MemoryStream())
            {
                var features = new FeatureCollection();
                features[typeof(IHttpResponseFeature)] = new HttpResponseFeature
                {
                    Body = body
                };

                features[typeof(IHttpRequestFeature)] = new HttpRequestFeature
                {
                    Headers = new HeaderDictionary()
                };

                var resource = new Resource<TestResource>
                {
                    Data = new TestResource
                    {
                        Content = "HAL-lo there"
                    }
                };

                var serviceProvider = new ServiceCollection()
                .AddLogging(logging =>
                {
                    logging.AddDebug();
                })
                .BuildServiceProvider();

                var httpContext = new DefaultHttpContext(features)
                {
                    RequestServices = serviceProvider
                };

                var writeContext = new OutputFormatterWriteContext(
                    httpContext,
                    (s, e) => new StreamWriter(s, e, 4096, true),
                    resource.GetType(),
                    resource);

                await formatter.WriteAsync(writeContext);

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
