using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// A simple convention scanner that scans using the provided assembly candidate finder
    /// Implements the <see cref="ConventionScannerBase" />
    /// </summary>
    /// <seealso cref="ConventionScannerBase" />
    public class SimpleConventionScanner : ConventionScannerBase
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
        public SimpleConventionScanner(
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceProvider serviceProvider,
            ILogger logger
        )
            : base(assemblyCandidateFinder, serviceProvider, logger) { }

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
        internal SimpleConventionScanner(
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