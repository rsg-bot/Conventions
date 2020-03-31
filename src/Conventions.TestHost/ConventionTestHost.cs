using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Configuration;
using Rocket.Surgery.Extensions.DependencyInjection;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

#pragma warning disable IDE0058 // Expression value is never used

namespace Rocket.Surgery.Conventions.TestHost
{
    /// <summary>
    /// Class ConventionTestHostBuilder.
    /// Implements the <see cref="ConventionHostBuilder{ConventionTestHostBuilder}" />
    /// </summary>
    /// <seealso cref="ConventionHostBuilder{ConventionTestHostBuilder}" />
    public class ConventionTestHost : ConventionHostBuilder<ConventionTestHost>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IHostEnvironment _environment;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionTestHost" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="serviceProperties">The service properties.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="environment">The environment.</param>
        internal ConventionTestHost(
            IConventionScanner scanner,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IAssemblyProvider assemblyProvider,
            DiagnosticSource diagnosticSource,
            IServiceProviderDictionary serviceProperties,
            ILoggerFactory loggerFactory,
            IHostEnvironment environment
        ) : base(scanner, assemblyCandidateFinder, assemblyProvider, diagnosticSource, serviceProperties)
        {
            serviceProperties.Set(HostType.UnitTestHost);
            _loggerFactory = loggerFactory;
            _environment = environment;
            _logger = ServiceProperties.Get<ILogger>();
        }

        /// <summary>
        /// Use the <see cref="BasicConventionScanner" /> to not automatically load conventions from attributes.
        /// </summary>
        /// <returns></returns>
        public ConventionTestHost ExcludeConventionAttributes() => new ConventionTestHost(
            new BasicConventionScanner(ServiceProperties),
            AssemblyCandidateFinder,
            AssemblyProvider,
            DiagnosticSource,
            ServiceProperties,
            _loggerFactory,
            _environment
        );

        /// <summary>
        /// <para>Use the <see cref="SimpleConventionScanner" /> to automatically load conventions from attributes.</para>
        /// <para>This is the default</para>
        /// </summary>
        public ConventionTestHost IncludeConventionAttributes() => new ConventionTestHost(
            new SimpleConventionScanner(AssemblyCandidateFinder, ServiceProperties, _logger),
            AssemblyCandidateFinder,
            AssemblyProvider,
            DiagnosticSource,
            ServiceProperties,
            _loggerFactory,
            _environment
        );

        /// <summary>
        /// Build the configuration and populate the service collection based on the input environment.
        /// </summary>
        private (IConfigurationRoot Configuration, ServicesBuilder ServicesBuilder) Init()
        {
            var sharedConfiguration = this.Get<IConfiguration>();
            if (sharedConfiguration == null)
            {
                var configurationOptions = this.GetOrAdd(() => new ConfigOptions());
                var sharedConfigurationBuilder = new ConfigurationBuilder()
                   .SetFileProvider(_environment.ContentRootFileProvider)
                   .Apply(configurationOptions.ApplicationConfiguration)
                   .Apply(configurationOptions.EnvironmentConfiguration, _environment.EnvironmentName)
                   .Apply(configurationOptions.EnvironmentConfiguration, "local");
                sharedConfiguration = sharedConfigurationBuilder.Build();
            }

#pragma warning disable CA2000
            var cb = new ConfigBuilder(
                Scanner,
                _environment,
                new ConfigurationRoot(new List<IConfigurationProvider>()),
                _logger,
                ServiceProperties
            );
#pragma warning restore CA2000

            
            var configuration = new ConfigurationBuilder()
               .AddConfiguration(sharedConfiguration, false)
               .AddConfiguration(cb.Build(), true)
               .Build();

            var serviceCollection = new ServiceCollection();
            var servicesBuilder = new ServicesBuilder(
                Scanner,
                AssemblyProvider,
                AssemblyCandidateFinder,
                serviceCollection,
                configuration,
                _environment,
                _logger,
                ServiceProperties
            );

            servicesBuilder.Services.AddLogging(
                builder =>
                {
                    builder.ClearProviders();
                    builder.Services.AddSingleton(_loggerFactory);
                }
            );

            Composer.Register(
                servicesBuilder.Scanner,
                servicesBuilder,
                typeof(IServiceConvention),
                typeof(ServiceConventionDelegate)
            );

            return ( configuration, servicesBuilder );
        }

        /// <summary>
        /// Build the configuration and service provider based on the input environment.
        /// </summary>
        public (IConfigurationRoot Configuration, IServiceProvider ServiceProvider) Build()
        {
            var (configuration, servicesBuilder) = Init();

            var serviceProvider = servicesBuilder.Build();

            return ( configuration, serviceProvider );
        }

        /// <summary>
        /// Build the configuration and populate the service collection based on the input environment.
        /// </summary>
        public (IConfigurationRoot Configuration, IServiceCollection ServiceCollection) Parse()
        {
            var (configuration, servicesBuilder) = Init();

            Composer.Register(
                servicesBuilder.Scanner,
                servicesBuilder,
                typeof(IServiceConvention),
                typeof(ServiceConventionDelegate)
            );

            return ( configuration, servicesBuilder.Services );
        }
    }
}