using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Extensions.Configuration.Tests
{
    public class ConfigurationBuilderTests : AutoFakeTest
    {
        [Fact]
        public void Constructs()
        {
            var configuration = AutoFake.Resolve<IConfiguration>();
            var builder = AutoFake.Resolve<ConfigBuilder>();

            builder.Logger.Should().NotBeNull();
            builder.Configuration.Should().NotBeNull();

            builder.Configuration.Should().BeSameAs(configuration);
            Action a = () => { builder.AppendConvention(A.Fake<IConfigConvention>()); };
            a.Should().NotThrow();
            a = () => { builder.AppendDelegate(delegate { }); };
            a.Should().NotThrow();
        }

        [Fact]
        public void BuildsSafely()
        {
            var builder = AutoFake.Resolve<ConfigBuilder>();

            Action a = () => builder.Build();
            a.Should().NotThrow();
        }

        public ConfigurationBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }
}