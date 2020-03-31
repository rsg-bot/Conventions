using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class AppDomainAssemblyCandidateFinderTests : AutoFakeTest
    {
        [Fact]
        public void FindsAssembliesInCandidates_Params()
        {
            var resolver = new AppDomainAssemblyCandidateFinder(AppDomain.CurrentDomain, Logger);
            var items = resolver.GetCandidateAssemblies(
                    new[] { "Rocket.Surgery.Conventions", "Rocket.Surgery.Conventions.Abstractions", "Rocket.Surgery.Conventions.Attributes" }
                )
               .Select(x => x.GetName().Name)
               .ToArray();

            foreach (var item in items)
            {
                Logger.LogInformation(item);
            }

            items
               .Should()
               .Contain(
                    new[]
                    {
                        "Sample.DependencyOne",
                        //"Sample.DependencyTwo",
                        "Sample.DependencyThree",
                        "Rocket.Surgery.Conventions.Tests"
                    }
                );
            items
               .Last()
               .Should()
               .Be("Rocket.Surgery.Conventions.Tests");
        }

        [Fact]
        public void FindsAssembliesInCandidates_Empty()
        {
            var resolver = new AppDomainAssemblyCandidateFinder(AppDomain.CurrentDomain, Logger);
            var items = resolver.GetCandidateAssemblies(Array.Empty<string>().AsEnumerable())
               .Select(x => x.GetName().Name)
               .ToArray();

            foreach (var item in items)
            {
                Logger.LogInformation(item);
            }

            items.Should().BeEmpty();
        }

        public AppDomainAssemblyCandidateFinderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }
}