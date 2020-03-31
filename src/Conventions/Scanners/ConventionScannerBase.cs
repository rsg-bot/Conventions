using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Conventions.Scanners
{
    /// <summary>
    /// Base class for various scanners
    /// Implements the <see cref="IConventionScanner" />
    /// </summary>
    /// <seealso cref="IConventionScanner" />
    public abstract class ConventionScannerBase : IConventionScanner
    {
        private static readonly ConcurrentDictionary<Assembly, List<IConvention>> Conventions =
            new ConcurrentDictionary<Assembly, List<IConvention>>();

        private readonly List<object> _prependedConventions;
        private readonly List<object> _appendedConventions;
        private readonly List<Type> _exceptConventions;
        private readonly List<Assembly> _exceptAssemblyConventions;
        private readonly IAssemblyCandidateFinder _assemblyCandidateFinder;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private IConventionProvider? _provider;
        private bool _updatedConventionCollections;

        /// <summary>
        /// Default constructor for the scanner
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder</param>
        /// <param name="serviceProvider">
        /// The service provider for creating instances of conventions (usually a
        /// <see cref="IServiceProviderDictionary" />.
        /// </param>
        /// <param name="logger">A diagnostic logger</param>
        protected ConventionScannerBase(
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceProvider serviceProvider,
            ILogger logger
        )
        {
            _assemblyCandidateFinder = assemblyCandidateFinder ??
                throw new ArgumentNullException(nameof(assemblyCandidateFinder));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger;
            _prependedConventions = new List<object>();
            _appendedConventions = new List<object>();
            _exceptConventions = new List<Type>();
            _exceptAssemblyConventions = new List<Assembly>();
        }

        internal ConventionScannerBase(
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceProvider serviceProvider,
            ILogger logger,
            List<object> prependedConventions,
            List<object> appendedConventions,
            List<Type> exceptConventions,
            List<Assembly> exceptAssemblyConventions
        )
        {
            _assemblyCandidateFinder = assemblyCandidateFinder ??
                throw new ArgumentNullException(nameof(assemblyCandidateFinder));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger;
            _prependedConventions = prependedConventions;
            _appendedConventions = appendedConventions;
            _exceptConventions = exceptConventions;
            _exceptAssemblyConventions = exceptAssemblyConventions;
        }

        /// <summary>
        /// Gets all the assemblies by convention.
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<IConvention> GetAssemblyConventions()
        {
            var assemblies = _assemblyCandidateFinder.GetCandidateAssemblies(
                "Rocket.Surgery.Conventions.Abstractions",
                "Rocket.Surgery.Conventions.Attributes",
                "Rocket.Surgery.Conventions"
            ).ToArray();

            var prependedConventionTypes = new Lazy<HashSet<Type>>(
                () =>
                    new HashSet<Type>(_prependedConventions.Select(x => x is Type t ? t : x.GetType()).Distinct())
            );
            var appendedConventionTypes = new Lazy<HashSet<Type>>(
                () =>
                    new HashSet<Type>(_appendedConventions.Select(x => x is Type t ? t : x.GetType()).Distinct())
            );

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(
                    "Scanning for conventions in assemblies: {Assemblies}",
                    assemblies.Select(x => x.GetName().Name)
                );
                if (_exceptAssemblyConventions.Any())
                {
                    _logger.LogDebug(
                        "Skipping conventions in assemblies: {Assemblies}",
                        _exceptAssemblyConventions.Select(x => x.GetName().Name)
                    );
                }

                _logger.LogDebug(
                    "Skipping existing convention types: {Types}",
                    prependedConventionTypes.Value.Concat(appendedConventionTypes.Value).Select(x => x.FullName)
                );
            }

            foreach (var assembly in assemblies.Except(_exceptAssemblyConventions))
            {
                if (!Conventions.TryGetValue(assembly, out var types))
                {
                    types = assembly.GetCustomAttributes<ConventionAttribute>()
                       .Select(x => x.Type)
                       .Distinct()
                       .Select(type => ActivatorUtilities.CreateInstance(_serviceProvider, type))
                       .Cast<IConvention>()
                       .ToList();
                    Conventions.TryAdd(assembly, types);
                }
                else if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(
                        "Conventions from Assembly {Assembly} have already been scanned and activated!",
                        assembly.GetName().Name
                    );
                }

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug(
                        "Found conventions in Assembly {Assembly} ({@Conventions})",
                        assembly.GetName().Name,
                        types.Select(z => z.GetType().FullName)
                    );
                }

                foreach (var item in types
                   .Select(
                        x =>
                        {
                            if (_logger.IsEnabled(LogLevel.Trace))
                            {
                                _logger.LogTrace(
                                    "Scanning => Prefilter: {Assembly} / {Type}",
                                    assembly.GetName().Name,
                                    x.GetType().FullName
                                );
                            }

                            return x;
                        }
                    )
                   .Where(
                        type => !prependedConventionTypes.Value.Contains(type.GetType()) &&
                            !appendedConventionTypes.Value.Contains(type.GetType())
                    )
                   .Select(
                        x =>
                        {
                            if (_logger.IsEnabled(LogLevel.Trace))
                            {
                                _logger.LogTrace(
                                    "Scanning => Postfilter: {Assembly} / {Type}",
                                    assembly.GetName().Name,
                                    x.GetType().FullName
                                );
                            }

                            return x;
                        }
                    ))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Method used to create a convention provider
        /// </summary>
        /// <returns></returns>
        private IConventionProvider CreateProvider()
        {
            if (!_updatedConventionCollections)
            {
                for (var i = 0; i < _prependedConventions.Count; i++)
                {
                    if (_prependedConventions[i] is Type type)
                    {
                        _prependedConventions[i] = ActivatorUtilities.CreateInstance(_serviceProvider, type);
                    }
                }

                for (var i = 0; i < _appendedConventions.Count; i++)
                {
                    if (_appendedConventions[i] is Type type)
                    {
                        _appendedConventions[i] = ActivatorUtilities.CreateInstance(_serviceProvider, type);
                    }
                }

                _updatedConventionCollections = true;
            }

            var contributionTypes = GetAssemblyConventions()
               .Where(z => _exceptConventions.All(x => x != z.GetType()));

            return new ConventionProvider(contributionTypes, _prependedConventions, _appendedConventions);
        }

        /// <summary>
        /// Creates a provider that returns a set of conventions.
        /// </summary>
        /// <returns>IConventionProvider.</returns>
        /// <inheritdoc />
        public IConventionProvider BuildProvider() => _provider ??= CreateProvider();

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention(IEnumerable<IConvention> conventions)
        {
            _provider = null;
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention(IEnumerable<Type> conventions)
        {
            _provider = null;
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention(params IConvention[] conventions)
        {
            _provider = null;
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention(params Type[] conventions)
        {
            _provider = null;
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention<T>()
            where T : IConvention
        {
            _provider = null;
            _appendedConventions.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention(IEnumerable<IConvention> conventions)
        {
            _provider = null;
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention(IEnumerable<Type> conventions)
        {
            _provider = null;
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention(params IConvention[] conventions)
        {
            _provider = null;
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention(params Type[] conventions)
        {
            _provider = null;
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention<T>()
            where T : IConvention
        {
            _provider = null;
            _prependedConventions.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendDelegate(IEnumerable<Delegate> delegates)
        {
            _provider = null;
            _appendedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendDelegate(params Delegate[] delegates)
        {
            _provider = null;
            _appendedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependDelegate(IEnumerable<Delegate> delegates)
        {
            _provider = null;
            _prependedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependDelegate(params Delegate[] delegates)
        {
            _provider = null;
            _prependedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="types">The convention types to exclude.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner ExceptConvention(IEnumerable<Type> types)
        {
            _provider = null;
            _exceptConventions.AddRange(types);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="types">The additional types to exclude.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner ExceptConvention(params Type[] types)
        {
            _provider = null;
            _exceptConventions.AddRange(types);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="assemblies">The convention types to exclude.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner ExceptConvention(IEnumerable<Assembly> assemblies)
        {
            _provider = null;
            _exceptAssemblyConventions.AddRange(assemblies);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="assemblies">The additional types to exclude.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner ExceptConvention(params Assembly[] assemblies)
        {
            _provider = null;
            _exceptAssemblyConventions.AddRange(assemblies);
            return this;
        }
    }
}