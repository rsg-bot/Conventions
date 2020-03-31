using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Configuration;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.WebJobs;

namespace Rocket.Surgery.Hosting.Functions
{
    /// <summary>
    /// Class RocketFunctionHostBuilder.
    /// Implements the <see cref="ConventionHostBuilder{IRocketFunctionHostBuilder}" />
    /// Implements the <see cref="IRocketFunctionHostBuilder" />
    /// </summary>
    /// <seealso cref="ConventionHostBuilder{IRocketFunctionHostBuilder}" />
    /// <seealso cref="IRocketFunctionHostBuilder" />
    internal class RocketFunctionHostBuilder : ConventionHostBuilder<IRocketFunctionHostBuilder>,
                                               IRocketFunctionHostBuilder
    {
        private static IHostEnvironment CreateEnvironment()
        {
            var environmentNames = new[]
            {
                Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT"),
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                Environment.GetEnvironmentVariable("WEBSITE_SLOT_NAME"),
                "Production"
            };

            var applicationNames = new[]
            {
                Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"),
                "Functions"
            };

            return new HostEnvironment()
            {
                ApplicationName = applicationNames.First(x => !string.IsNullOrEmpty(x)),
                EnvironmentName = environmentNames.First(x => !string.IsNullOrEmpty(x)),
                ContentRootPath = null,
                ContentRootFileProvider = null
            };
        }

        private readonly object _startupInstance;
        private readonly IHostEnvironment _environment;
        private readonly DiagnosticLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RocketFunctionHostBuilder" /> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="functionsAssembly">The functions assembly.</param>
        /// <param name="startupInstance">The startup instance.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="serviceProviderDictionary">The service provider dictionary of values</param>
        public RocketFunctionHostBuilder(
            IWebJobsBuilder builder,
            Assembly functionsAssembly,
            object startupInstance,
            IHostEnvironment environment,
            IConventionScanner scanner,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IAssemblyProvider assemblyProvider,
            DiagnosticSource diagnosticSource,
            IServiceProviderDictionary serviceProviderDictionary
        ) : base(scanner, assemblyCandidateFinder, assemblyProvider, diagnosticSource, serviceProviderDictionary)
        {
            _startupInstance = startupInstance;
            _environment = environment ?? CreateEnvironment();
            _logger = new DiagnosticLogger(DiagnosticSource);
            Builder = builder;
            FunctionsAssembly = functionsAssembly;
        }

        /// <summary>
        /// Withes the specified scanner.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <returns>RocketFunctionHostBuilder.</returns>
        internal RocketFunctionHostBuilder With(IConventionScanner scanner) => new RocketFunctionHostBuilder(
            Builder,
            FunctionsAssembly,
            _startupInstance,
            _environment,
            scanner,
            AssemblyCandidateFinder,
            AssemblyProvider,
            DiagnosticSource,
            ServiceProperties
        );

        /// <summary>
        /// Withes the specified assemby.
        /// </summary>
        /// <param name="assemby">The assemby.</param>
        /// <returns>RocketFunctionHostBuilder.</returns>
        internal RocketFunctionHostBuilder With(Assembly assemby) => new RocketFunctionHostBuilder(
            Builder,
            assemby,
            _startupInstance,
            _environment,
            Scanner,
            AssemblyCandidateFinder,
            AssemblyProvider,
            DiagnosticSource,
            ServiceProperties
        );

        /// <summary>
        /// Withes the specified assembly candidate finder.
        /// </summary>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <returns>RocketFunctionHostBuilder.</returns>
        internal RocketFunctionHostBuilder With(IAssemblyCandidateFinder assemblyCandidateFinder)
            => new RocketFunctionHostBuilder(
                Builder,
                FunctionsAssembly,
                _startupInstance,
                _environment,
                Scanner,
                assemblyCandidateFinder,
                AssemblyProvider,
                DiagnosticSource,
                ServiceProperties
            );

        /// <summary>
        /// Withes the specified assembly provider.
        /// </summary>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <returns>RocketFunctionHostBuilder.</returns>
        internal RocketFunctionHostBuilder With(IAssemblyProvider assemblyProvider) => new RocketFunctionHostBuilder(
            Builder,
            FunctionsAssembly,
            _startupInstance,
            _environment,
            Scanner,
            AssemblyCandidateFinder,
            assemblyProvider,
            DiagnosticSource,
            ServiceProperties
        );

        /// <summary>
        /// Withes the specified diagnostic source.
        /// </summary>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>RocketFunctionHostBuilder.</returns>
        internal RocketFunctionHostBuilder With(DiagnosticSource diagnosticSource) => new RocketFunctionHostBuilder(
            Builder,
            FunctionsAssembly,
            _startupInstance,
            _environment,
            Scanner,
            AssemblyCandidateFinder,
            AssemblyProvider,
            diagnosticSource,
            ServiceProperties
        );

        /// <summary>
        /// Withes the specified environment.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <returns>RocketFunctionHostBuilder.</returns>
        internal RocketFunctionHostBuilder With(IHostEnvironment environment) => new RocketFunctionHostBuilder(
            Builder,
            FunctionsAssembly,
            _startupInstance,
            environment,
            Scanner,
            AssemblyCandidateFinder,
            AssemblyProvider,
            DiagnosticSource,
            ServiceProperties
        );

        private IConfiguration SetupConfiguration()
        {
            var currentDirectory = Environment.GetEnvironmentVariable("AzureWebJobsScriptRoot") ?? "/home/site/wwwroot";
            var isLocal = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")) &&
                !Directory.Exists(currentDirectory);
            if (isLocal)
            {
                currentDirectory = Environment.CurrentDirectory;
            }

            var existingConfiguration = Builder.Services.First(z => z.ServiceType == typeof(IConfiguration))
               .ImplementationInstance as IConfiguration;

            var configurationOptions = this.GetOrAdd(() => new ConfigOptions());

            var configurationBuilder = new ConfigurationBuilder()
               .SetBasePath(currentDirectory)
               .AddConfiguration(existingConfiguration, false)
               .Apply(configurationOptions.ApplicationConfiguration)
               .Apply(configurationOptions.EnvironmentConfiguration, _environment.EnvironmentName)
               .Apply(configurationOptions.EnvironmentConfiguration, "local");

            if (_environment.IsDevelopment())
            {
                configurationBuilder.AddUserSecrets(FunctionsAssembly, true);
            }

            configurationBuilder
               .AddEnvironmentVariables("RSG_")
               .AddEnvironmentVariables();

            IConfigurationSource? source = null;
            foreach (var item in configurationBuilder.Sources.Reverse())
            {
                if (( item is EnvironmentVariablesConfigurationSource env && ( string.IsNullOrWhiteSpace(env.Prefix) ||
                        string.Equals(env.Prefix, "RSG_", StringComparison.OrdinalIgnoreCase) ) ) ||
                    ( item is JsonConfigurationSource a && string.Equals(
                        a.Path,
                        "secrets.json",
                        StringComparison.OrdinalIgnoreCase
                    ) ))
                {
                    continue;
                }

                source = item;
                break;
            }

            var index = source == null
                ? configurationBuilder.Sources.Count - 1
                : configurationBuilder.Sources.IndexOf(source);

            var cb = new ConfigBuilder(
                Scanner,
                _environment,
                new ConfigurationBuilder()
                   .AddConfiguration(existingConfiguration, false)
                   .AddConfiguration(configurationBuilder.Build(), true)
                   .Build(),
                _logger,
                Properties
            );

            configurationBuilder.Sources.Insert(
                index + 1,
                new ChainedConfigurationSource
                {
                    Configuration = cb.Build(),
                    ShouldDisposeConfiguration = true
                }
            );

            var newConfig = configurationBuilder.Build();

            Builder.Services.Replace(ServiceDescriptor.Singleton<IConfiguration>(newConfig));
            return newConfig;
        }

        private void SetupServices(IConfiguration existingConfiguration)
        {
            var builder = new ServicesBuilder(
                Scanner,
                AssemblyProvider,
                AssemblyCandidateFinder,
                Builder.Services,
                existingConfiguration,
                _environment,
                _logger,
                Properties
            );

            Composer.Register<IServiceConventionContext, IServiceConvention, ServiceConventionDelegate>(
                Scanner.BuildProvider(),
                builder
            );
        }

        private void SetupWebJobs(IConfiguration existingConfiguration)
        {
            var builder = new WebJobsConventionBuilder(
                Scanner,
                AssemblyProvider,
                AssemblyCandidateFinder,
                Builder,
                existingConfiguration,
                _environment,
                _logger,
                Properties
            );

            builder.Build();
        }

        /// <summary>
        /// Composes this instance.
        /// </summary>
        public void Compose()
        {
            var existingHostedServices = Builder.Services.Where(x => x.ServiceType == typeof(IHostedService)).ToArray();
            if (_startupInstance is IConvention convention)
            {
                Scanner.AppendConvention(convention);
            }

            var configuration = SetupConfiguration();
            SetupServices(configuration);
            SetupWebJobs(configuration);

            Builder.Services.RemoveAll<IHostedService>();
            Builder.Services.Add(existingHostedServices);
        }

        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <value>The builder.</value>
        public IWebJobsBuilder Builder { get; }

        /// <summary>
        /// Gets the functions assembly.
        /// </summary>
        /// <value>The functions assembly.</value>
        public Assembly FunctionsAssembly { get; }
    }
}