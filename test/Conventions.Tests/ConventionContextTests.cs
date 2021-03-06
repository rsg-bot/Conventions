using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionContextTests : AutoFakeTest
    {
        [Fact]
        public void ReturnsNullOfNoValue()
        {
            var container = new TestGenericValueContainer(Logger);

            container[typeof(string)].Should().BeNull();
        }


        [Fact]
        public void SetAValue()
        {
            var container = new TestGenericValueContainer(Logger);

            container[typeof(string)] = "abc";

            container[typeof(string)].Should().Be("abc");
        }

        [Fact]
        public void GetAStronglyTypedValue()
        {
            var container = new TestGenericValueContainer(Logger);
            container[typeof(string)] = "abc";
            container.Get<string>().Should().Be("abc");
        }

        [Fact]
        public void SetAStronglyTypedValue()
        {
            var container = new TestGenericValueContainer(Logger);
            container.Set("abc");
            container.Get<string>().Should().Be("abc");
        }

        public ConventionContextTests(ITestOutputHelper outputHelper) : base(outputHelper) { }

        private class TestGenericValueContainer : ConventionContext
        {
            public TestGenericValueContainer(ILogger logger) : base(logger, new Dictionary<object, object?>()) { }
        }
    }
}