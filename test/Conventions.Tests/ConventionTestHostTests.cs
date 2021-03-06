using System;
using System.Diagnostics;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Configuration.Ini;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using NetEscapades.Configuration.Yaml;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.TestHost;
using Rocket.Surgery.Extensions.Configuration;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionTestHostTests : AutoFakeTest
    {
        [Fact]
        public void Builder_Should_Create_Host()
        {
            Action a = () => ConventionTestHostBuilder.For(this, LoggerFactory)
               .Create();
            a.Should().NotThrow();
        }


        [Fact]
        public void Builder_Should_Create_With_Delegate()
        {
            Action a = () => ConventionTestHostBuilder.For(this, LoggerFactory)
               .Create(builder => { });
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Not_Create_Host_When_Missing_AssemblyCandidateFinder()
        {
            Action a = () => new ConventionTestHostBuilder().With(A.Fake<IAssemblyProvider>()).Create();
            a.Should().Throw<ArgumentNullException>().Where(x => x.ParamName == "AssemblyCandidateFinder");
        }

        [Fact]
        public void Builder_Should_Not_Create_Host_When_Missing_AssemblyProvider()
        {
            Action a = () => new ConventionTestHostBuilder().With(A.Fake<IAssemblyCandidateFinder>()).Create();
            a.Should().Throw<ArgumentNullException>().Where(x => x.ParamName == "AssemblyProvider");
        }

        [Fact]
        public void Builder_Should_Create_Host_ByType()
        {
            Action a = () => ConventionTestHostBuilder.For(GetType(), LoggerFactory)
               .Create();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Create_Host_ByAssembly()
        {
            Action a = () => ConventionTestHostBuilder.For(GetType().Assembly, LoggerFactory)
               .Create();
            a.Should().NotThrow();
        }

        [Fact]
        [Obsolete("This functionality will be removed in the future")]
        public void Builder_Should_Create_Host_ByDependencyContext()
        {
            Action a = () => ConventionTestHostBuilder.For(DependencyContext.Load(GetType().Assembly), LoggerFactory)
               .Create();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Build_Host()
        {
            Action a = () => ConventionTestHostBuilder.For(this, LoggerFactory)
               .Create()
               .Build();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Parse_Host()
        {
            Action a = () => ConventionTestHostBuilder.For(this, LoggerFactory)
               .Create()
               .Parse();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_IConventionScanner()
        {
            var builder = ConventionTestHostBuilder.For(this, LoggerFactory)
               .With(AutoFake.Resolve<IConventionScanner>())
               .Create();

            builder.Scanner.Should().BeSameAs(AutoFake.Resolve<IConventionScanner>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_IAssemblyCandidateFinder()
        {
            var builder = ConventionTestHostBuilder.For(this, LoggerFactory)
               .With(AutoFake.Resolve<IAssemblyCandidateFinder>())
               .Create();

            builder.AssemblyCandidateFinder.Should().BeSameAs(AutoFake.Resolve<IAssemblyCandidateFinder>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_IAssemblyProvider()
        {
            var builder = ConventionTestHostBuilder.For(this, LoggerFactory)
               .With(AutoFake.Resolve<IAssemblyProvider>())
               .Create();

            builder.AssemblyProvider.Should().BeSameAs(AutoFake.Resolve<IAssemblyProvider>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_IServiceProviderDictionary()
        {
            var builder = ConventionTestHostBuilder.For(this, LoggerFactory)
               .With(AutoFake.Resolve<IServiceProviderDictionary>())
               .Create();

            builder.ServiceProperties.Should().BeSameAs(AutoFake.Resolve<IServiceProviderDictionary>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_DiagnosticSource()
        {
            var builder = ConventionTestHostBuilder.For(this, LoggerFactory)
               .With(AutoFake.Resolve<DiagnosticSource>())
               .Create();

            builder.DiagnosticSource.Should().BeSameAs(AutoFake.Resolve<DiagnosticSource>());
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_ILogger()
        {
            Action a = () => ConventionTestHostBuilder.For(this, LoggerFactory)
               .With(AutoFake.Resolve<ILogger>())
               .Create();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Use_A_Custom_IRocketEnvironment()
        {
            Action a = () => ConventionTestHostBuilder.For(this, LoggerFactory)
               .With(AutoFake.Resolve<IRocketEnvironment>())
               .Create();
            a.Should().NotThrow();
        }

        [Fact]
        public void Builder_Should_Scan_For_Conventions_When_Desired()
        {
            var host = ConventionTestHostBuilder.For(this, LoggerFactory)
               .Create()
               .IncludeConventionAttributes();

            host.Scanner.BuildProvider().GetAll().Should().NotBeEmpty();
        }

        [Fact]
        public void Builder_Should_Not_Scan_For_Conventions()
        {
            var host = ConventionTestHostBuilder.For(this, LoggerFactory)
               .Create()
               .ExcludeConventionAttributes();

            host.Scanner.BuildProvider().GetAll().Should().BeEmpty();
        }

        [Fact]
        public void Builder_Should_Add_Configuration()
        {
            var host = ConventionTestHostBuilder.For(this, LoggerFactory)
               .Create();

            var (configuration, _) = host.Build();

            configuration.Providers.OfType<JsonConfigurationProvider>().Should().HaveCount(3);
            configuration.Providers.OfType<YamlConfigurationProvider>().Should().HaveCount(6);
            configuration.Providers.OfType<IniConfigurationProvider>().Should().HaveCount(3);
        }

        [Fact]
        public void Builder_Should_Add_Configuration_After_It_Has_Been_Changed()
        {
            var host = ConventionTestHostBuilder.For(this, LoggerFactory)
               .Create();
            var options = host.GetOrAdd(() => new ConfigurationOptions());

            options.EnvironmentConfiguration.Clear();

            var (configuration, _) = host.Build();

            configuration.Providers.OfType<JsonConfigurationProvider>().Should().HaveCount(1);
            configuration.Providers.OfType<YamlConfigurationProvider>().Should().HaveCount(2);
            configuration.Providers.OfType<IniConfigurationProvider>().Should().HaveCount(1);
        }

        public ConventionTestHostTests(ITestOutputHelper outputHelper) : base(outputHelper, LogLevel.Information) { }
    }
}