﻿using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Rocket.Surgery.Extensions.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Rocket.Surgery.Conventions.Tests
{
    public class ConventionTests : AutoTestBase
    {
        public ConventionTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void ConventionAttributeThrowsIfNonConventionGiven()
        {
            Action a = () => new ConventionAttribute(typeof(object));
            a.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void ComposerCallsValuesAsExpected()
        {
            var scanner = A.Fake<IConventionScanner>();
            var provider = A.Fake<IConventionProvider>();

            var contrib = A.Fake<IServiceConvention>();
            var contrib2 = A.Fake<IServiceConvention>();
            var dele = A.Fake<ServiceConventionDelegate>();
            var dele2 = A.Fake<ServiceConventionDelegate>();

            A.CallTo(() => scanner.BuildProvider()).Returns(provider);
            A.CallTo(() => provider.Get<IServiceConvention, ServiceConventionDelegate>())
                .Returns(new[]
                {
                    new DelegateOrConvention(contrib),
                    new DelegateOrConvention(contrib2),
                    new DelegateOrConvention(dele),
                    new DelegateOrConvention(dele2),
                }.AsEnumerable());
            var composer = new ServiceConventionComposer(scanner);

            composer.Register(new ServiceConventionContext(A.Fake<IRocketEnvironment>(), Logger));
            composer.Register(new ServiceConventionContext(A.Fake<IRocketEnvironment>(), Logger));
            A.CallTo(() => dele.Invoke(A<ServiceConventionContext>._)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => dele2.Invoke(A<ServiceConventionContext>._)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => contrib.Register(A<ServiceConventionContext>._)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => contrib2.Register(A<ServiceConventionContext>._)).MustHaveHappenedTwiceExactly();
        }
    }
}
