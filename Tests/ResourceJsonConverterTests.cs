using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Passless.AspNetCore.Hal;
using Passless.AspNetCore.Hal.Converters;
using Passless.AspNetCore.Hal.Models;

namespace Tests
{
    public class ResourceJsonConverterTests
    {
        class TestResource
        {
            public string Currency { get; set; }
            public string Status { get; set; }
            public decimal Total { get; set; }
        }

        public class InheritTestResource : Resource
        {
            public string Currency { get; set; }
            public string Status { get; set; }
            public decimal Total { get; set; }
        }

        [SetUp]
        public void Setup()
        {
        }

        //[Test]
        //public void DeserializeGenericResource()
        //{
        //    string json = "{ \"_links\": { \"self\": { \"href\": \"/orders/523\" }, \"warehouse\": { \"href\": \"/warehouse/56\" }, \"invoice\": { \"href\": \"/invoices/873\" } }, \"currency\": \"USD\", \"status\": \"shipped\", \"total\": 10.20 }";
        //    var serializer = new JsonSerializer();
        //    var mappingConverter = new MappingJsonConverter();
        //    mappingConverter.Mappings.Add(typeof(ILink), typeof(Link));
        //    serializer.Converters.Add(mappingConverter);
        //    var resourceConverter = new ResourceJsonConverter();
        //    serializer.Converters.Add(resourceConverter);

        //    using (var textReader = new StringReader(json))
        //    using (var jsonReader = new JsonTextReader(textReader))
        //    {
        //        var resource = serializer.Deserialize<Resource<TestResource>>(jsonReader);

        //        Assert.IsNotNull(resource);
        //        Assert.IsNotNull(resource.Data);
        //        Assert.AreEqual("USD", resource.Data.Currency);
        //        Assert.AreEqual("shipped", resource.Data.Status);
        //        Assert.AreEqual(10.20, resource.Data.Total);
        //        Assert.AreEqual(3, resource.Links.Count);
        //        Assert.AreEqual(0, resource.Embedded.Count);

        //        var links = resource.Links.ToList();
        //        Assert.AreEqual("self", links[0].Rel);
        //        Assert.AreEqual("/orders/523", links[0].HRef);

        //        Assert.AreEqual("warehouse", links[1].Rel);
        //        Assert.AreEqual("/warehouse/56", links[1].HRef);

        //        Assert.AreEqual("invoice", links[2].Rel);
        //        Assert.AreEqual("/invoices/873", links[2].HRef);
        //    }
        //}

        //[Test]
        //public void DeserializeNonGenericResource()
        //{
        //    string json = "{ \"_links\": { \"self\": { \"href\": \"/orders/523\" }, \"warehouse\": { \"href\": \"/warehouse/56\" }, \"invoice\": { \"href\": \"/invoices/873\" } }, \"currency\": \"USD\", \"status\": \"shipped\", \"total\": 10.20 }";
        //    Console.WriteLine(json);
        //    var serializer = new JsonSerializer();
        //    var mappingConverter = new MappingJsonConverter();
        //    mappingConverter.Mappings.Add(typeof(ILink), typeof(Link));
        //    serializer.Converters.Add(mappingConverter);
        //    var resourceConverter = new ResourceJsonConverter();
        //    serializer.Converters.Add(resourceConverter);

        //    using (var textReader = new StringReader(json))
        //    using (var jsonReader = new JsonTextReader(textReader))
        //    {
        //        var resource = serializer.Deserialize<InheritTestResource>(jsonReader);

        //        Assert.IsNotNull(resource);
        //        Assert.AreEqual("USD", resource.Currency);
        //        Assert.AreEqual("shipped", resource.Status);
        //        Assert.AreEqual(10.20, resource.Total);
        //        Assert.AreEqual(3, resource.Links.Count);
        //        Assert.AreEqual(0, resource.Embedded.Count);

        //        var links = resource.Links.ToList();
        //        Assert.AreEqual("self", links[0].Rel);
        //        Assert.AreEqual("/orders/523", links[0].HRef);

        //        Assert.AreEqual("warehouse", links[1].Rel);
        //        Assert.AreEqual("/warehouse/56", links[1].HRef);

        //        Assert.AreEqual("invoice", links[2].Rel);
        //        Assert.AreEqual("/invoices/873", links[2].HRef);
        //    }
        //}

        [Test]
        public void SerializeGenericResource()
        {
            string expected = "{\"_links\":{\"self\":{\"href\":\"/orders/523\"},\"warehouse\":{\"href\":\"/warehouse/56\"},\"invoice\":{\"href\":\"/invoices/873\"}},\"currency\":\"USD\",\"status\":\"shipped\",\"total\":10.20}"; 
            var serializer = new JsonSerializer();
            serializer.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var resourceConverter = new ResourceJsonConverter();
            serializer.Converters.Add(resourceConverter);

            var resource = new Resource<TestResource>(
                new TestResource
                {
                    Currency = "USD",
                    Status = "shipped",
                    Total = 10.20M
                })
            {
                Links = new List<ILink>
                {
                    new Link
                    {
                        Rel = "self",
                        HRef = "/orders/523"
                    },
                    new Link
                    {
                        Rel = "warehouse",
                        HRef = "/warehouse/56"
                    },
                    new Link
                    {
                        Rel = "invoice",
                        HRef = "/invoices/873"
                    }
                },
                SingularRelations = new HashSet<string>
                {
                    "self",
                    "warehouse",
                    "invoice"
                }
            };

            
            using (var textWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(textWriter))
            {
                serializer.Serialize(jsonWriter, resource);
                Assert.AreEqual(expected, textWriter.ToString());
            }
        }

        [Test]
        public void SerializeNonGenericResource()
        {
            string expected = "{\"_links\":{\"self\":{\"href\":\"/orders/523\"},\"warehouse\":{\"href\":\"/warehouse/56\"},\"invoice\":{\"href\":\"/invoices/873\"}},\"currency\":\"USD\",\"status\":\"shipped\",\"total\":10.20}";
            var serializer = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var resourceConverter = new ResourceJsonConverter();
            serializer.Converters.Add(resourceConverter);

            var resource = new InheritTestResource
            { 
                Currency = "USD",
                Status = "shipped",
                Total = 10.20M,
                Links = new List<ILink>
                {
                    new Link
                    {
                        Rel = "self",
                        HRef = "/orders/523"
                    },
                    new Link
                    {
                        Rel = "warehouse",
                        HRef = "/warehouse/56"
                    },
                    new Link
                    {
                        Rel = "invoice",
                        HRef = "/invoices/873"
                    }
                },
                SingularRelations = new HashSet<string>
                {
                    "self",
                    "warehouse",
                    "invoice"
                }
            };


            using (var textWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(textWriter))
            {
                serializer.Serialize(jsonWriter, resource);
                Assert.AreEqual(expected, textWriter.ToString());
            }
        }
    }
}
