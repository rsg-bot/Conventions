using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Helper method for working with <see cref="IConventionHostBuilder" />
    /// </summary>
    public static class ConfigHostBuilderExtensions
    {
        /// <summary>
        /// Configure the configuration delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder ConfigureConfiguration(
            [NotNull] this IConventionHostBuilder container,
            ConfigConventionDelegate @delegate
        )
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Scanner.AppendDelegate(@delegate);
            return container;
        }

        /// <summary>
        /// Configures the application configuration.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="options"></param>
        public static IHostBuilder ConfigureEnhancedConfiguration(this IHostBuilder hostBuilder) => ConfigureEnhancedConfiguration(hostBuilder, new ConfigOptions());

        /// <summary>
        /// Configures the application configuration.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="options"></param>
        public static IHostBuilder ConfigureEnhancedConfiguration(
            this IHostBuilder hostBuilder,
            ConfigOptions options
        ) => ConfigureEnhancedConfiguration(hostBuilder, () => options);

        /// <summary>
        /// Configures the application configuration.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configOptionsAction"></param>
        public static IHostBuilder ConfigureEnhancedConfiguration(
            this IHostBuilder hostBuilder,
            Func<ConfigOptions> configOptionsAction
        ) => ConfigureEnhancedConfiguration(hostBuilder, _ => configOptionsAction());

        /// <summary>
        /// Configures the application configuration.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configOptionsAction"></param>
        public static IHostBuilder ConfigureEnhancedConfiguration(
            this IHostBuilder hostBuilder,
            Action<ConfigOptions> configOptionsAction
        ) => ConfigureEnhancedConfiguration(
            hostBuilder,
            _ =>
            {
                configOptionsAction(_);
                return _;
            }
        );

        /// <summary>
        /// Configures the application configuration.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="configOptionsAction"></param>
        public static IHostBuilder ConfigureEnhancedConfiguration(
            this IHostBuilder hostBuilder,
            Func<ConfigOptions, ConfigOptions> configOptionsAction
        )
        {
            hostBuilder.ConfigureAppConfiguration(
                (context, configurationBuilder) =>
                {
                    var options = configOptionsAction(new ConfigOptions());
                    InsertConfigurationSourceAfter(
                        configurationBuilder.Sources,
                        sources => sources.OfType<JsonConfigurationSource>().FirstOrDefault(
                            x =>
                                string.Equals(
                                    x.Path,
                                    $"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                                    StringComparison.OrdinalIgnoreCase
                                )
                        ),
                        new IConfigurationSource[]
                        {
                            new JsonConfigurationSource
                            {
                                FileProvider = configurationBuilder.GetFileProvider(),
                                Path = "appsettings.local.json",
                                Optional = true,
                                ReloadOnChange = true
                            }
                        }
                    );

                    ReplaceConfigurationSourceAt(
                        configurationBuilder.Sources,
                        sources => sources.OfType<JsonConfigurationSource>().FirstOrDefault(
                            x => string.Equals(x.Path, "appsettings.json", StringComparison.OrdinalIgnoreCase)
                        ),
                        new ProxyConfigurationBuilder(configurationBuilder).Apply(options.ApplicationConfiguration)
                           .GetAdditionalSources()
                    );

                    ReplaceConfigurationSourceAt(
                        configurationBuilder.Sources,
                        sources => sources.OfType<JsonConfigurationSource>().FirstOrDefault(
                            x =>
                                string.Equals(
                                    x.Path,
                                    $"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                                    StringComparison.OrdinalIgnoreCase
                                )
                        ),
                        new ProxyConfigurationBuilder(configurationBuilder).Apply(
                            options.EnvironmentConfiguration,
                            context.HostingEnvironment.EnvironmentName
                        ).GetAdditionalSources()
                    );

                    ReplaceConfigurationSourceAt(
                        configurationBuilder.Sources,
                        sources => sources.OfType<JsonConfigurationSource>().FirstOrDefault(
                            x =>
                                string.Equals(x.Path, "appsettings.local.json", StringComparison.OrdinalIgnoreCase)
                        ),
                        new ProxyConfigurationBuilder(configurationBuilder)
                           .Apply(options.EnvironmentConfiguration, "local").GetAdditionalSources()
                    );

                    InsertConfigurationSourceBefore(
                        configurationBuilder.Sources,
                        sources => sources.OfType<EnvironmentVariablesConfigurationSource>()
                           .FirstOrDefault(x => string.IsNullOrWhiteSpace(x.Prefix)),
                        s => new EnvironmentVariablesConfigurationSource
                        {
                            Prefix = "RSG_"
                        }
                    );

                    IConfigurationSource? source = null;
                    foreach (var item in configurationBuilder.Sources.Reverse())
                    {
                        if (item is CommandLineConfigurationSource ||
                            ( item is EnvironmentVariablesConfigurationSource env &&
                                ( string.IsNullOrWhiteSpace(env.Prefix) ||
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
                }
            );

            return hostBuilder;
        }

        private static void InsertConfigurationSourceAfter<T>(
            IList<IConfigurationSource> sources,
            Func<IList<IConfigurationSource>, T> getSource,
            IEnumerable<IConfigurationSource> createSourceFrom
        )
            where T : IConfigurationSource
        {
            var source = getSource(sources);
            if (source != null)
            {
                var index = sources.IndexOf(source);
                foreach (var newSource in createSourceFrom.Reverse())
                {
                    sources.Insert(index + 1, newSource);
                }
            }
        }

        private static void ReplaceConfigurationSourceAt<T>(
            IList<IConfigurationSource> sources,
            Func<IList<IConfigurationSource>, T> getSource,
            IEnumerable<IConfigurationSource> createSourceFrom
        )
            where T : IConfigurationSource
        {
            var source = getSource(sources);
            if (source != null)
            {
                var index = sources.IndexOf(source);
                sources.RemoveAt(index);
                foreach (var newSource in createSourceFrom.Reverse())
                {
                    sources.Insert(index, newSource);
                }
            }
        }

        private static void InsertConfigurationSourceBefore<T>(
            IList<IConfigurationSource> sources,
            Func<IList<IConfigurationSource>, T> getSource,
            params Func<T, IConfigurationSource>[] createSourceFrom
        )
            where T : IConfigurationSource
        {
            var source = getSource(sources);
            if (source != null)
            {
                var index = sources.IndexOf(source);
                foreach (var m in createSourceFrom.Reverse())
                {
                    sources.Insert(index, m(source));
                }
            }
        }
    }
}