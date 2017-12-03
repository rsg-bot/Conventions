﻿using System;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Conventions.Tests.Fixtures;
using Xunit;

namespace Rocket.Surgery.Conventions.Tests
{
    public class GenericConventionComposerTests
    {

        public class TestConventionComposer : ConventionComposer<ITestConventionContext, ITestConvention,
            TestConventionDelegate>
        {
            public TestConventionComposer(IConventionScanner scanner) : base(scanner)
            {
            }
        }

        public class NotADelegateComposer : ConventionComposer<ITestConventionContext, ITestConvention, string>
        {
            public NotADelegateComposer(IConventionScanner scanner) : base(scanner)
            {
            }
        }

        [Fact]
        public void ShouldConstructComposer()
        {
            var scanner = A.Fake<IConventionScanner>();
            var composer = new TestConventionComposer(scanner);

            composer.Should().NotBeNull();
        }

        [Fact]
        public void ShouldThrowIfDelegateIsNotADelegate()
        {
            var scanner = A.Fake<IConventionScanner>();
            Action action = () => new NotADelegateComposer(scanner);
            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void RegisterShouldCallConvention()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var contribution = A.Fake<ITestConvention>();
            var context = A.Fake<ITestConventionContext>();
            var composer = new TestConventionComposer(scanner);

            A.CallTo(() => scanner.BuildProvider())
                .Returns(scannerProvider);
            A.CallTo(() => scannerProvider.Get<ITestConvention, TestConventionDelegate>())
                .Returns(new[] {new DelegateOrConvention(contribution)});

            composer.Register(context);

            A.CallTo(() => contribution.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void RegisterShouldCallConventions()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var contribution1 = A.Fake<ITestConvention>();
            var contribution2 = A.Fake<ITestConvention>();
            var contribution3 = A.Fake<ITestConvention>();
            var context = A.Fake<ITestConventionContext>();
            var composer = new TestConventionComposer(scanner);

            A.CallTo(() => scanner.BuildProvider())
                .Returns(scannerProvider);
            A.CallTo(() => scannerProvider.Get<ITestConvention, TestConventionDelegate>())
                .Returns(new[]
                {
                    new DelegateOrConvention(contribution1),
                    new DelegateOrConvention(contribution2),
                    new DelegateOrConvention(contribution3)
                });

            composer.Register(context);

            A.CallTo(() => contribution1.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => contribution2.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => contribution3.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void RegisterShouldCallDelegate()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var @delegate = A.Fake<TestConventionDelegate>();
            var context = A.Fake<ITestConventionContext>();
            var composer = new TestConventionComposer(scanner);

            A.CallTo(() => scanner.BuildProvider()).Returns(scannerProvider);
            // A.CallTo(() => scannerProvider.Get<ITestConvention>()).Returns(new[] { convention });
            A.CallTo(() => scannerProvider.Get<ITestConvention, TestConventionDelegate>())
                .Returns(new DelegateOrConvention[] {@delegate});

            composer.Register(context);

            A.CallTo(() => @delegate.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void RegisterShouldCallDelegates()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var delegate1 = A.Fake<TestConventionDelegate>();
            var delegate2 = A.Fake<TestConventionDelegate>();
            var delegate3 = A.Fake<TestConventionDelegate>();
            var context = A.Fake<ITestConventionContext>();
            var composer = new TestConventionComposer(scanner);

            A.CallTo(() => scanner.BuildProvider()).Returns(scannerProvider);
            // A.CallTo(() => scannerProvider.Get<ITestConvention>()).Returns(new[] { convention });
            A.CallTo(() => scannerProvider.Get<ITestConvention, TestConventionDelegate>())
                .Returns(new[]
                {
                    new DelegateOrConvention(delegate2),
                    new DelegateOrConvention(delegate1),
                    new DelegateOrConvention(delegate3)
                });

            composer.Register(context);

            A.CallTo(() => @delegate1.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => @delegate2.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => @delegate3.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void RegisterShouldCallConventionsAndDelegates()
        {
            var scanner = A.Fake<IConventionScanner>();
            var scannerProvider = A.Fake<IConventionProvider>();
            var contribution = A.Fake<ITestConvention>();
            var @delegate = A.Fake<TestConventionDelegate>();
            var context = A.Fake<ITestConventionContext>();
            var composer = new TestConventionComposer(scanner);

            A.CallTo(() => scanner.BuildProvider()).Returns(scannerProvider);
            // A.CallTo(() => scannerProvider.Get<ITestConvention>()).Returns(new[] { convention });
            A.CallTo(() => scannerProvider.Get<ITestConvention, TestConventionDelegate>())
                .Returns(new[]
                {
                    new DelegateOrConvention(@delegate),
                    new DelegateOrConvention(contribution)
                });

            composer.Register(context);

            A.CallTo(() => contribution.Register(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => @delegate.Invoke(A<ITestConventionContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
