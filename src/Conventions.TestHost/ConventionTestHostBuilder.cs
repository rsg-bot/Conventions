using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Configuration;

namespace Rocket.Surgery.Conventions.TestHost
{
    /// <summary>
    /// A convention test host builder
    /// </summary>
    public class ConventionTestHostBuilder
    {
        public static ConditionalWeakTable<object, IConfiguration> _sharedConfigurations = new ConditionalWeakTable<object, IConfiguration>();
        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="type">The type that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static ConventionTestHostBuilder For([NotNull] Type type, ILoggerFactory? loggerFactory = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return For(
                DependencyContext.Load(type.Assembly),
                type.Assembly,
                loggerFactory
            );
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="instance">The object that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static ConventionTestHostBuilder For([NotNull] object instance, ILoggerFactory? loggerFactory = null)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            return For(
                DependencyContext.Load(instance.GetType().Assembly),
                instance.GetType().Assembly,
                loggerFactory
            );
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static ConventionTestHostBuilder For([NotNull] Assembly assembly, ILoggerFactory? loggerFactory = null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return For(
                DependencyContext.Load(assembly),
                assembly,
                loggerFactory
            );
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="context">The context that that will be used for the test host.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        [Obsolete(
            "No longer to be used, has been replaced with the overload that takes the assembly and loads the context from that"
        )]
        public static ConventionTestHostBuilder For(DependencyContext context, ILoggerFactory? loggerFactory = null)
        {
            loggerFactory ??= NullLoggerFactory.Instance;
            var logger = loggerFactory.CreateLogger(nameof(ConventionTestHostBuilder));
            return new ConventionTestHostBuilder()
                   .With(loggerFactory)
                   .With(new DependencyContextAssemblyProvider(context, logger))
                   .With(new DependencyContextAssemblyCandidateFinder(context, logger))
                ;
        }

        /// <summary>
        /// Create a convention test host build for the given <see cref="DependencyContext" /> in the assembly.
        /// </summary>
        /// <param name="context">The context that that will be used for the test host.</param>
        /// <param name="assembly">The assembly that that will be used to load the <see cref="DependencyContext" />.</param>
        /// <param name="loggerFactory">Optional logger factory.</param>
        public static ConventionTestHostBuilder For(
            DependencyContext context,
            Assembly assembly,
            ILoggerFactory? loggerFactory = null
        )
        {
            loggerFactory ??= NullLoggerFactory.Instance;
            var logger = loggerFactory.CreateLogger(nameof(ConventionTestHostBuilder));
            return new ConventionTestHostBuilder()
                   .With(loggerFactory)
                   .With(assembly)
                   .With(new DependencyContextAssemblyProvider(context, logger))
                   .With(new DependencyContextAssemblyCandidateFinder(context, logger))
                ;
        }

        private IServiceProviderDictionary _serviceProperties = new ServiceProviderDictionary();
        private IConventionScanner? _scanner;
        private IAssemblyCandidateFinder? _assemblyCandidateFinder;
        private IAssemblyProvider? _assemblyProvider;
        private ILoggerFactory? _loggerFactory;
        private ILogger? _logger;
        private DiagnosticSource? _diagnosticSource;
        private IHostEnvironment? _environment;
        private Assembly? _assembly;
        private IConfiguration _reuseConfiguration;
        private object _sharedConfigurationKey;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ConventionTestHostBuilder() { }

        /// <summary>
        /// Use the specific <see cref="IConventionScanner" />
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <returns>ConventionTestHostBuilder.</returns>
        public ConventionTestHostBuilder With(IConventionScanner scanner)
        {
            _scanner = scanner;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IConventionScanner" />
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <returns>ConventionTestHostBuilder.</returns>
        public ConventionTestHostBuilder WithScanner(IConventionScanner scanner) => With(scanner);

        /// <summary>
        /// Use the specific <see cref="IAssemblyCandidateFinder" />
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        public ConventionTestHostBuilder With(IAssemblyCandidateFinder assemblyCandidateFinder)
        {
            _assemblyCandidateFinder = assemblyCandidateFinder;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IAssemblyCandidateFinder" />
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        public ConventionTestHostBuilder WithAssemblyCandidateFinder(IAssemblyCandidateFinder assemblyCandidateFinder)
            => With(assemblyCandidateFinder);

        /// <summary>
        /// Use the specific <see cref="Assembly" />
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public ConventionTestHostBuilder With(Assembly assembly)
        {
            _assembly = assembly;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="Assembly" />
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public ConventionTestHostBuilder WithAssembly(Assembly assembly) => With(assembly);

        /// <summary>
        /// Use the specific <see cref="IAssemblyProvider" />
        /// </summary>
        /// <param name="assemblyProvider">The assembly provider.</param>
        public ConventionTestHostBuilder With(IAssemblyProvider assemblyProvider)
        {
            _assemblyProvider = assemblyProvider;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IAssemblyProvider" />
        /// </summary>
        /// <param name="assemblyProvider">The assembly provider.</param>
        public ConventionTestHostBuilder WithAssemblyProvider(IAssemblyProvider assemblyProvider)
            => With(assemblyProvider);

        /// <summary>
        /// Use the specific <see cref="IServiceProviderDictionary" />
        /// </summary>
        /// <param name="serviceProperties">The service provider dictionary.</param>
        public ConventionTestHostBuilder With(IServiceProviderDictionary serviceProperties)
        {
            _serviceProperties = serviceProperties;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IServiceProviderDictionary" />
        /// </summary>
        /// <param name="serviceProperties">The service provider dictionary.</param>
        public ConventionTestHostBuilder WithServiceProperties(IServiceProviderDictionary serviceProperties)
            => With(serviceProperties);

        /// <summary>
        /// Use the specific <see cref="ILogger" />
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ConventionTestHostBuilder With(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="ILogger" />
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ConventionTestHostBuilder WithLogger(ILogger logger) => With(logger);

        /// <summary>
        /// Use the specific <see cref="ILoggerFactory" />
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public ConventionTestHostBuilder With(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="ILoggerFactory" />
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public ConventionTestHostBuilder WithLoggerFactory(ILoggerFactory loggerFactory) => With(loggerFactory);

        /// <summary>
        /// Use the specific <see cref="ILogger" />
        /// </summary>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        public ConventionTestHostBuilder With(DiagnosticSource diagnosticSource)
        {
            _diagnosticSource = diagnosticSource;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="ILogger" />
        /// </summary>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        public ConventionTestHostBuilder WithDiagnosticSource(DiagnosticSource diagnosticSource)
            => With(diagnosticSource);

        /// <summary>
        /// Use the specific <see cref="IHostEnvironment" />
        /// </summary>
        /// <param name="environment">The environment.</param>
        public ConventionTestHostBuilder With(IHostEnvironment environment)
        {
            _environment = environment;
            return this;
        }

        /// <summary>
        /// Use the specific <see cref="IHostEnvironment" />
        /// </summary>
        /// <param name="environment">The environment.</param>
        public ConventionTestHostBuilder WithEnvironment(IHostEnvironment environment) => With(environment);

        /// <summary>
        /// Use the specific <see cref="IConventionScanner" />
        /// </summary>
        /// <param name="sharedConfiguration">The shared configuration.</param>
        /// <returns>ConventionTestHostBuilder.</returns>
        public ConventionTestHostBuilder With(IConfiguration sharedConfiguration)
        {
            _reuseConfiguration = sharedConfiguration;
            return this;
        }

        /// <summary>
        /// Use a specific configuration object with the test host
        /// (This can help avoid re-reading the same configuration over and over)
        /// </summary>
        /// <param name="sharedConfiguration">The shared configuration.</param>
        /// <returns>ConventionTestHostBuilder.</returns>
        public ConventionTestHostBuilder WithConfiguration(IConfiguration sharedConfiguration)
            => With(sharedConfiguration);

        /// <summary>
        /// Use a specific configuration object with the test host
        /// (This can help avoid re-reading the same configuration over and over)
        /// </summary>
        /// <param name="key">The object to use as a key for shared configuration</param>
        /// <returns>ConventionTestHostBuilder.</returns>
        public ConventionTestHostBuilder ShareConfiguration(object key)
        {
            _sharedConfigurationKey = key;
            return this;
        }

        /// <summary>
        /// Create the convention test host with the given defaults
        /// </summary>
        /// <returns></returns>
        public ConventionTestHost Create() => Create(_ => { });

        /// <summary>
        /// Create the convention test host with the given defaults
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public ConventionTestHost Create([NotNull] Action<IConventionHostBuilder> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
#pragma warning disable CA2208
            // ReSharper disable NotResolvedInText
            var assemblyCandidateFinder =
                _assemblyCandidateFinder ?? throw new ArgumentNullException("AssemblyCandidateFinder");
            var assemblyProvider = _assemblyProvider ?? throw new ArgumentNullException("AssemblyProvider");
            // ReSharper restore NotResolvedInText
#pragma warning restore CA2208

            var contentRootPath = _assembly != null && Directory.Exists(Path.GetDirectoryName(_assembly.Location))
                ? Path.GetDirectoryName(_assembly.Location)
                : string.Empty;
            
            var contentProvider = !string.IsNullOrWhiteSpace(contentRootPath)
                ? new PhysicalFileProvider(contentRootPath)
                : new NullFileProvider() as IFileProvider;

            var environment = _environment ?? new HostEnvironment()
            {
                ApplicationName = nameof(ConventionTestHost),
                EnvironmentName = "Test",
                ContentRootPath = contentRootPath,
                ContentRootFileProvider = contentProvider
            };
            
            var builder = new ConventionTestHost(
                _scanner ?? new SimpleConventionScanner(
                    assemblyCandidateFinder,
                    _serviceProperties,
                    _logger ?? NullLogger.Instance
                ),
                assemblyCandidateFinder,
                assemblyProvider,
                _diagnosticSource ?? new DiagnosticListener(nameof(ConventionTestHost)),
                _serviceProperties,
                _loggerFactory ?? NullLoggerFactory.Instance,
                environment
            );
            if (_reuseConfiguration != null)
            {
                builder.Set(_reuseConfiguration);
            }
            else if (_sharedConfigurationKey != null)
            {
                if (!_sharedConfigurations.TryGetValue(_sharedConfigurationKey, out var configuration))
                {
                    var options = new ConfigOptions();
                    configuration = new ConfigurationBuilder()
                       .SetFileProvider(environment.ContentRootFileProvider)
                       .Apply(options.ApplicationConfiguration)
                       .Apply(options.EnvironmentConfiguration, environment.EnvironmentName)
                       .Apply(options.EnvironmentConfiguration, "local")
                       .Build();
                    _sharedConfigurations.Add(_sharedConfigurationKey, configuration);
                }
                builder.Set(configuration);
            }
            action(builder);
            return builder;
        }
    }
}