using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class DependencyContextAssemblyCandidateFinderTests : AutoFakeTest
    {
        [Fact]
        public void FindsAssembliesInCandidates_Params()
        {
            var resolver = new DependencyContextAssemblyCandidateFinder(
                DependencyContext.Load(typeof(DependencyContextAssemblyCandidateFinderTests).GetTypeInfo().Assembly),
                Logger
            );
            var items = resolver.GetCandidateAssemblies(
                    "Rocket.Surgery.Conventions",
                    "Rocket.Surgery.Conventions.Abstractions",
                    "Rocket.Surgery.Conventions.Attributes"
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
                        "Sample.DependencyTwo",
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
        public void FindsAssembliesInCandidates_Enumerable()
        {
            var resolver = new DependencyContextAssemblyCandidateFinder(
                DependencyContext.Load(typeof(DependencyContextAssemblyCandidateFinderTests).GetTypeInfo().Assembly),
                Logger
            );
            var items = resolver.GetCandidateAssemblies(
                    new[] { "Rocket.Surgery.Conventions", "Rocket.Surgery.Conventions.Abstractions",
                        "Rocket.Surgery.Conventions.Attributes" }.AsEnumerable()
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
                        "Sample.DependencyTwo",
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
        public void FindsAssembliesInCandidates_Params_Multiples()
        {
            var resolver = new DependencyContextAssemblyCandidateFinder(
                DependencyContext.Load(typeof(DependencyContextAssemblyCandidateFinderTests).GetTypeInfo().Assembly),
                Logger
            );
            var items = resolver.GetCandidateAssemblies(
                    new[] { "Rocket.Surgery.Conventions", "Rocket.Surgery.Conventions.Abstractions",
                        "Rocket.Surgery.Conventions.Attributes" }
                )
               .Select(x => x.GetName().Name)
               .ToArray();
            var items2 = resolver.GetCandidateAssemblies(
                    "Rocket.Surgery.Conventions",
                    "Rocket.Surgery.Conventions.Abstractions",
                    "Rocket.Surgery.Conventions.Attributes"
                )
               .Select(x => x.GetName().Name)
               .ToArray();

            foreach (var item in items)
            {
                Logger.LogInformation(item);
            }

            foreach (var item in items2)
            {
                Logger.LogInformation(item);
            }

            items
               .Should()
               .Contain(
                    new[]
                    {
                        "Sample.DependencyOne",
                        "Sample.DependencyTwo",
                        "Sample.DependencyThree",
                        "Rocket.Surgery.Conventions.Tests"
                    }
                );
            items
               .Last()
               .Should()
               .Be("Rocket.Surgery.Conventions.Tests");
            items.Should().BeEquivalentTo(items2);
        }

        [Fact]
        public void FindsAssembliesInCandidates_Empty()
        {
            var resolver = new DependencyContextAssemblyCandidateFinder(
                DependencyContext.Load(typeof(DependencyContextAssemblyCandidateFinderTests).GetTypeInfo().Assembly),
                Logger
            );
            var items = resolver.GetCandidateAssemblies(Array.Empty<string>().AsEnumerable())
               .Select(x => x.GetName().Name)
               .ToArray();

            foreach (var item in items)
            {
                Logger.LogInformation(item);
            }

            items.Should().BeEmpty();
        }

        public DependencyContextAssemblyCandidateFinderTests(ITestOutputHelper outputHelper) : base(outputHelper) { }
    }
}