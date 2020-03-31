using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// A convention provider that is bootstrapped with a set of conventions that allow for manual inalization of the initial
    /// conventions.
    /// Implements the <see cref="ConventionScannerBase" />
    /// </summary>
    /// <seealso cref="ConventionScannerBase" />
    public class AggregateConventionScanner : ConventionScannerBase
    {
        /// <summary>
        /// The default constructor
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder</param>
        /// <param name="serviceProvider">
        /// The service provider for creating instances of conventions (usually a
        /// <see cref="IServiceProviderDictionary" />.
        /// </param>
        /// <param name="logger">A diagnostic logger</param>
        /// <param name="conventions">The additional conventions to start with</param>
        public AggregateConventionScanner(
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceProvider serviceProvider,
            ILogger logger,
            params IConvention[] conventions
        )
            : base(assemblyCandidateFinder, serviceProvider, logger) => PrependConvention(conventions);

        /// <summary>
        /// This constructor is used to forward information captured from a <see cref="UninitializedConventionScanner" />
        /// </summary>
        /// <param name="assemblyCandidateFinder"></param>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        /// <param name="prependedConventions"></param>
        /// <param name="appendedConventions"></param>
        /// <param name="exceptConventions"></param>
        /// <param name="exceptAssemblyConventions"></param>
        internal AggregateConventionScanner(
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceProvider serviceProvider,
            ILogger logger,
            List<object> prependedConventions,
            List<object> appendedConventions,
            List<Type> exceptConventions,
            List<Assembly> exceptAssemblyConventions
        ) : base(
            assemblyCandidateFinder,
            serviceProvider,
            logger,
            prependedConventions,
            appendedConventions,
            exceptConventions,
            exceptAssemblyConventions
        ) { }
    }
}