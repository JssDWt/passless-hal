using System;
using System.Collections.Generic;
using NUnit.Framework;
using Passless.AspNetCore.Hal;
using Passless.AspNetCore.Hal.Extensions;
using Passless.AspNetCore.Hal.Models;

namespace Tests
{
    public class TestResource
    {
        public string Content { get; set; }
    }

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test, Sequential]
        public void IsOfGenericTypeTest(
            [Values(typeof(List<string>), typeof(Resource<TestResource>))]Type type,
            [Values(typeof(List<>), typeof(Resource<>))]Type genericType)
        {
            bool result = type.IsOfGenericType(genericType);
            Assert.IsTrue(result);
        }

        [Test, Sequential]
        public void IsOfGenericType_NegativeTest(
            [Values(typeof(List<string>), typeof(List<string>), typeof(ICollection<string>), typeof(Resource<TestResource>))]Type type,
            [Values(typeof(Dictionary<,>), typeof(List<string>), typeof(List<string>), typeof(Resource<string>))]Type genericType)
        {
            bool result = type.IsOfGenericType(genericType);
            Assert.IsFalse(result);
        }

        [Test, Sequential]
        public void GetGenericArgumentsTest(
            [Values(typeof(List<TestResource>), typeof(Resource<TestResource>))]Type type,
            [Values(typeof(List<>), typeof(Resource<>))]Type genericType)
        {
            Type[] result = type.GetGenericArguments(genericType);
            CollectionAssert.AreEqual(new Type[] { typeof(TestResource) }, result);
        }
    }
}