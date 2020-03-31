using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Configuration;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Testing;
using Rocket.Surgery.Extensions.WebJobs;
using Xunit;
using Xunit.Abstractions;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace Rocket.Surgery.Hosting.Functions.Tests
{
    internal class WebJobsBuilder : IWebJobsBuilder
    {
        public WebJobsBuilder()
        {
            Services = new ServiceCollection();
            Services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
        }

        public IServiceCollection Services { get; }
    }

    internal class Startup : IServiceConvention
    {
        public void Register(IServiceConventionContext context) => context.Services.AddSingleton(context);
    }

    public class RocketHostBuilderTests : AutoFakeTest
    {
        [Fact]
        public void Should_Call_Through_To_Delegate_Methods()
        {
            var startup = new Startup();
            new WebJobsBuilder()
               .UseRocketBooster(
                    startup,
                    RocketBooster.For(DependencyContext.Load(typeof(RocketHostTests).Assembly)),
                    rb => rb
                       .UseScanner(AutoFake.Resolve<IConventionScanner>())
                       .PrependDelegate(new Action(() => { }))
                       .AppendDelegate(new Action(() => { }))
                );

            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().PrependDelegate(A<Delegate>._))
               .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().AppendDelegate(A<Delegate>._))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact]
        public void Should_Call_Through_To_Convention_Methods()
        {
            var convention = AutoFake.Resolve<IConvention>();
            var startup = new Startup();
            new WebJobsBuilder()
               .UseRocketSurgery(
                    startup,
                    rb => rb
                       .UseScanner(AutoFake.Resolve<IConventionScanner>())
                       .PrependConvention(convention)
                       .AppendConvention(convention)
                );
            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().PrependConvention(A<IConvention>._))
               .MustHaveHappened(1, Times.Exactly);
            A.CallTo(() => AutoFake.Resolve<IConventionScanner>().AppendConvention(A<IConvention>._))
               .MustHaveHappened(1, Times.Exactly);
        }

        [Fact(Skip = "Disabled because it sometimes fails on windows")]
        public void Should_UseAppDomain()
        {
            var startup = new Startup();
            var builder = new WebJobsBuilder()
               .UseRocketSurgery(
                    startup,
                    rb => rb
                       .UseAppDomain(AppDomain.CurrentDomain)
                );

            var sp = builder.Services.BuildServiceProvider();
            sp.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseAssemblies()
        {
            var startup = new Startup();
            var builder = new WebJobsBuilder()
               .UseRocketSurgery(
                    startup,
                    rb => rb
                       .UseAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                );

            var sp = builder.Services.BuildServiceProvider();
            sp.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseRocketBooster()
        {
            var startup = new Startup();
            var builder = new WebJobsBuilder()
               .UseRocketBooster(startup, RocketBooster.For(AppDomain.CurrentDomain), rb => { });

            var sp = builder.Services.BuildServiceProvider();
            sp.Should().NotBeNull();
        }

        [Fact]
        public void Should_UseDependencyContext()
        {
            var startup = new Startup();
            var builder = new WebJobsBuilder()
               .UseRocketSurgery(
                    startup,
                    rb => rb
                       .UseDependencyContext(DependencyContext.Default)
                );

            var sp = builder.Services.BuildServiceProvider();
            sp.Should().NotBeNull();
        }

        [Fact]
        public void Should_Build_The_Host_Correctly()
        {
            var serviceConventionFake = A.Fake<IServiceConvention>();
            var configurationConventionFake = A.Fake<IConfigConvention>();

            var startup = new Startup();
            var builder = new WebJobsBuilder()
               .UseRocketSurgery(
                    startup,
                    rb => rb
                       .UseScanner(
                            new BasicConventionScanner(
                                A.Fake<IServiceProviderDictionary>(),
                                serviceConventionFake,
                                configurationConventionFake
                            )
                        )
                       .UseAssemblyCandidateFinder(
                            new DefaultAssemblyCandidateFinder(new[] { typeof(RocketHostBuilderTests).Assembly })
                        )
                       .UseAssemblyProvider(
                            new DefaultAssemblyProvider(new[] { typeof(RocketHostBuilderTests).Assembly })
                        )
                );

            var sp = builder.Services.BuildServiceProvider();
            sp.Should().NotBeNull();
        }

        [Fact]
        public void Should_ConfigureWebJobs()
        {
            var startup = new Startup();
            new WebJobsBuilder()
               .UseRocketSurgery(
                    startup,
                    rb => rb
                       .UseDependencyContext(DependencyContext.Default)
                       .UseScanner(AutoFake.Resolve<IConventionScanner>())
                       .ConfigureWebJobs(x => { })
                );

            A.CallTo(
                () => AutoFake.Resolve<IConventionScanner>().AppendDelegate(
                    A<Delegate[]>.That.Matches(z => z[0].GetType() == typeof(WebJobsConventionDelegate))
                )
            ).MustHaveHappened();
        }

        public RocketHostBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }
}