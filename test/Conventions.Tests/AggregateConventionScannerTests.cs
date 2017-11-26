﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FakeItEasy;
using FluentAssertions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Xunit;

namespace Rocket.Surgery.Conventions.Tests
{
    public class AggregateConventionScannerTests
    {
        [Fact]
        public void ShouldConstruct()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new AggregateConventionScanner(finder);

            scanner.Should().NotBeNull();
        }

        [Fact]
        public void ShouldBuildAProvider()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new AggregateConventionScanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new[] { typeof(ConventionScannerTests).GetTypeInfo().Assembly });

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .Contain(x => x.GetType() == typeof(Contrib));
        }

        [Fact]
        public void ShouldCacheTheProvider()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new AggregateConventionScanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new[] { typeof(ConventionScannerTests).GetTypeInfo().Assembly });

            var provider = scanner.BuildProvider();
            var provider2 = scanner.BuildProvider();

            provider.Should().BeSameAs(provider2);
        }

        [Fact]
        public void ShouldScanAddedContributions()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new AggregateConventionScanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            var contribution = A.Fake<IServiceConvention>();

            scanner.AddConvention(contribution);

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .Contain(contribution);
        }

        [Fact]
        public void ShouldIncludeAddedDelegates()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new AggregateConventionScanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            var @delegate = new ServiceConventionDelegate(context => { });

            scanner.AddDelegate(@delegate);

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Delegate)
                .Should()
                .Contain(@delegate);
        }

        [Fact]
        public void ShouldScanExcludeContributionTypes()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new AggregateConventionScanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            var contribution = A.Fake<IServiceConvention>();

            scanner.AddConvention(contribution);
            scanner.ExceptConvention(typeof(ConventionScannerTests));

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .NotContain(x => x.GetType() == typeof(Contrib));
        }

        [Fact]
        public void ShouldScanExcludeContributionAssemblies()
        {
            var finder = A.Fake<IAssemblyCandidateFinder>();
            var scanner = new AggregateConventionScanner(finder);

            A.CallTo(() => finder.GetCandidateAssemblies(A<IEnumerable<string>>._))
                .Returns(new Assembly[0]);

            var contribution = A.Fake<IServiceConvention>();

            scanner.AddConvention(contribution);
            scanner.ExceptConvention(typeof(ConventionScannerTests).GetTypeInfo().Assembly);

            var provider = scanner.BuildProvider();

            provider.Get<IServiceConvention, ServiceConventionDelegate>()
                .Select(x => x.Convention)
                .Should()
                .NotContain(x => x.GetType() == typeof(Contrib));
        }
    }
}
