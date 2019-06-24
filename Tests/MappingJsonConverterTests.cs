using System;
using System.IO;
using Newtonsoft.Json;
using NUnit.Framework;
using Passless.AspNetCore.Hal;
using Passless.AspNetCore.Hal.Converters;
using Passless.AspNetCore.Hal.Models;

namespace Tests
{
    public class MappingJsonConverterTests
    {
        [Test]
        public void LinkMappingTest()
        {
            var json = "{ \"href\": \"/orders\" }";
            var serializer = new JsonSerializer();
            var mappingConverter = new MappingJsonConverter();
            mappingConverter.Mappings.Add(typeof(ILink), typeof(Link));
            serializer.Converters.Add(mappingConverter);
            using (var textReader = new StringReader(json))
            using (var reader = new JsonTextReader(textReader))
            {
                var link = serializer.Deserialize<ILink>(reader);
                Assert.IsInstanceOf(typeof(Link), link);
                Assert.IsNotNull(link.HRef);
                Assert.AreEqual("/orders", link.HRef);
            }
        }
    }
}
