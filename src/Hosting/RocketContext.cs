using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Extensions.CommandLine;
using Rocket.Surgery.Extensions.Configuration;
using Rocket.Surgery.Extensions.DependencyInjection;

namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// Class RocketContext.
    /// </summary>
    internal class RocketContext
    {
        private readonly IHostBuilder _hostBuilder;
        private string[]? _args;
        private ICommandLineExecutor? _exec;

        /// <summary>
        /// Initializes a new instance of the <see cref="RocketContext" /> class.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        public RocketContext(IHostBuilder hostBuilder) => _hostBuilder = hostBuilder;

        /// <summary>
        /// Configures any hosting builder conventions
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        public void ComposeHostingConvention(IConfigurationBuilder configurationBuilder)
        {
            var rocketHostBuilder = _hostBuilder.GetConventions();
            Composer.Register(
                rocketHostBuilder.Scanner,
                new HostingConventionContext(rocketHostBuilder, rocketHostBuilder.Get<IHostBuilder>(), rocketHostBuilder.Get<ILogger>()),
                typeof(IHostingConvention),
                typeof(HostingConventionDelegate)
            );
        }

        /// <summary>
        /// Configures the cli.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        public void ConfigureCli(IConfigurationBuilder configurationBuilder)
        {
            var rocketHostBuilder = _hostBuilder.GetConventions();
            var clb = new CommandLineBuilder(
                rocketHostBuilder.Scanner,
                rocketHostBuilder.AssemblyProvider,
                rocketHostBuilder.AssemblyCandidateFinder,
                rocketHostBuilder.Get<ILogger>(),
                rocketHostBuilder.ServiceProperties
            );

            _exec = clb.Build().Parse(_args ?? Array.Empty<string>());
            _args = _exec.ApplicationState.RemainingArguments ?? Array.Empty<string>();
            configurationBuilder.AddApplicationState(_exec.ApplicationState);
            rocketHostBuilder.ServiceProperties.Add(typeof(ICommandLineExecutor), _exec);
        }

        /// <summary>
        /// Captures the arguments.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        public void CaptureArguments(IConfigurationBuilder configurationBuilder)
        {
            var commandLineSource = configurationBuilder.Sources.OfType<CommandLineConfigurationSource>()
               .FirstOrDefault();
            if (commandLineSource != null)
            {
                _args = commandLineSource.Args.ToArray();
            }
        }

        /// <summary>
        /// Replaces the arguments.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        public void ReplaceArguments(IConfigurationBuilder configurationBuilder)
        {
            var commandLineSource = configurationBuilder.Sources.OfType<CommandLineConfigurationSource>()
               .FirstOrDefault();
            if (commandLineSource != null)
            {
                commandLineSource.Args = _args;
            }
        }

        /// <summary>
        /// Configures the application configuration.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="configurationBuilder">The configuration builder.</param>
        public void ConfigureAppConfiguration(
            HostBuilderContext context,
            IConfigurationBuilder configurationBuilder
        )
        {
            var rocketHostBuilder = _hostBuilder.GetConventions();
            // Insert after all the normal configuration but before the environment specific configuration
            
            IConfigurationSource? source = null;
            foreach (var item in configurationBuilder.Sources.Reverse())
            {
                if (item is CommandLineConfigurationSource ||
                    (item is EnvironmentVariablesConfigurationSource env && (string.IsNullOrWhiteSpace(env.Prefix) ||
                        string.Equals(env.Prefix, "RSG_", StringComparison.OrdinalIgnoreCase))) ||
                    (item is JsonConfigurationSource a && string.Equals(
                        a.Path,
                        "secrets.json",
                        StringComparison.OrdinalIgnoreCase
                    )))
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
                rocketHostBuilder.Scanner,
                context.HostingEnvironment,
                new ConfigurationBuilder()
                   .AddConfiguration(context.Configuration, false)
                   .AddConfiguration(configurationBuilder.Build(), true)
                   .Build(),
                rocketHostBuilder.Get<ILogger>(),
                rocketHostBuilder.ServiceProperties
            );

            configurationBuilder.Sources.Insert(
                index + 1,
                new ChainedConfigurationSource
                {
                    Configuration = cb.Build(),
                    ShouldDisposeConfiguration = true
                }
            );
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="services">The services.</param>
        public void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var rocketHostBuilder = _hostBuilder.GetConventions();
            services.AddSingleton(rocketHostBuilder.AssemblyCandidateFinder);
            services.AddSingleton(rocketHostBuilder.AssemblyProvider);
            services.AddSingleton(rocketHostBuilder.Scanner);
            services.AddHealthChecks();
        }

        /// <summary>
        /// Defaults the services.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="services">The services.</param>
        public void DefaultServices(HostBuilderContext context, IServiceCollection services)
        {
            var conventionalBuilder = _hostBuilder.GetConventions();
            _hostBuilder.UseServiceProviderFactory(
                new ServicesBuilderServiceProviderFactory(
                    collection =>
                        new ServicesBuilder(
                            conventionalBuilder.Scanner,
                            conventionalBuilder.AssemblyProvider,
                            conventionalBuilder.AssemblyCandidateFinder,
                            collection,
                            context.Configuration,
                            context.HostingEnvironment,
                            conventionalBuilder.Get<ILogger>(),
                            conventionalBuilder.ServiceProperties
                        )
                )
            );
        }
    }
}