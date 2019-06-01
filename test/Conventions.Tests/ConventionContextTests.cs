using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionContextTests : AutoTestBase
    {
        public ConventionContextTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        class TestGenericValueContainer : ConventionContext
        {
            public TestGenericValueContainer(IRocketEnvironment environment, ILogger logger) : base(environment, logger, new Dictionary<object, object>())
            {
            }
        }

        [Fact]
        public void ReturnsNullOfNoValue()
        {
            var container = new TestGenericValueContainer(A.Fake<IRocketEnvironment>(), Logger);

            container[typeof(string)].Should().BeNull();
        }


        [Fact]
        public void SetAValue()
        {
            var container = new TestGenericValueContainer(A.Fake<IRocketEnvironment>(), Logger);

            container[typeof(string)] = "abc";

            container[typeof(string)].Should().Be("abc");
        }
        [Fact]
        public void GetAStronglyTypedValue()
        {
            var container = new TestGenericValueContainer(A.Fake<IRocketEnvironment>(), Logger);
            container[typeof(string)] = "abc";
            container.Get<string>().Should().Be("abc");
        }

        [Fact]
        public void SetAStronglyTypedValue()
        {
            var container = new TestGenericValueContainer(A.Fake<IRocketEnvironment>(), Logger);
            container.Set("abc");
            container.Get<string>().Should().Be("abc");
        }
    }
}
