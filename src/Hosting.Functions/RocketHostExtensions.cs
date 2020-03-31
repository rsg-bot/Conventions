using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.Configuration;

#pragma warning disable CA2000

//using Microsoft.Azure.WebJobs.Hosting;

namespace Rocket.Surgery.Hosting.Functions
{
    /// <summary>
    /// Class RocketHostExtensions.
    /// </summary>
    public static class RocketHostExtensions
    {
        /// <summary>
        /// Uses the rocket Surgery.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="startupInstance">The startup instance.</param>
        /// <param name="action">The action.</param>
        /// <returns>IWebJobsBuilder.</returns>
        public static IWebJobsBuilder UseRocketSurgery(
            this IWebJobsBuilder builder,
            [NotNull] object startupInstance,
            [NotNull] Action<IRocketFunctionHostBuilder> action
        )
        {
            if (startupInstance == null)
            {
                throw new ArgumentNullException(nameof(startupInstance));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var internalBuilder = GetOrCreateBuilder(builder, startupInstance, null);
            action(internalBuilder);
            internalBuilder.Compose();
            return builder;
        }

        /// <summary>
        /// Uses the rocket Surgery.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="startupInstance">The startup instance.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="action">The action.</param>
        /// <returns>IWebJobsBuilder.</returns>
        public static IWebJobsBuilder UseRocketSurgery(
            this IWebJobsBuilder builder,
            [NotNull] object startupInstance,
            [NotNull] IHostEnvironment environment,
            [NotNull] Action<IRocketFunctionHostBuilder> action
        )
        {
            if (startupInstance == null)
            {
                throw new ArgumentNullException(nameof(startupInstance));
            }

            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var internalBuilder = GetOrCreateBuilder(builder, startupInstance, environment);
            action(internalBuilder);
            internalBuilder.Compose();
            return builder;
        }

        /// <summary>
        /// Uses the rocket booster.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="startupInstance">The startup instance.</param>
        /// <param name="func">The function.</param>
        /// <param name="action">The action.</param>
        /// <returns>IWebJobsBuilder.</returns>
        /// <exception cref="Exception">Something bad happened...</exception>
        public static IWebJobsBuilder UseRocketBooster(
            this IWebJobsBuilder builder,
            [NotNull] object startupInstance,
            [NotNull] Func<IWebJobsBuilder, object, IRocketFunctionHostBuilder> func,
            [NotNull] Action<IRocketFunctionHostBuilder> action
        )
        {
            if (startupInstance == null)
            {
                throw new ArgumentNullException(nameof(startupInstance));
            }

            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var b = func(builder, startupInstance);
            action(b);
            if (!Builders.TryGetValue(builder, out var conventionalBuilder))
            {
                throw new Exception("Something bad happened...");
            }

            conventionalBuilder.Compose();
            return builder;
        }

        /// <summary>
        /// Uses the rocket booster.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="startupInstance">The startup instance.</param>
        /// <param name="func">The function.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="action">The action.</param>
        /// <returns>IWebJobsBuilder.</returns>
        /// <exception cref="Exception">Something bad happened...</exception>
        public static IWebJobsBuilder UseRocketBooster(
            this IWebJobsBuilder builder,
            [NotNull] object startupInstance,
            [NotNull] Func<IWebJobsBuilder, object, IRocketFunctionHostBuilder> func,
            [NotNull] IHostEnvironment environment,
            [NotNull] Action<IRocketFunctionHostBuilder> action
        )
        {
            if (startupInstance == null)
            {
                throw new ArgumentNullException(nameof(startupInstance));
            }

            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var b = func(builder, startupInstance);
            b.UseEnvironment(environment);
            action(b);
            if (!Builders.TryGetValue(builder, out var conventionalBuilder))
            {
                throw new Exception("Something bad happened...");
            }

            conventionalBuilder.Compose();
            return builder;
        }

        /// <summary>
        /// Launches the with.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="startupInstance">The startup instance.</param>
        /// <param name="func">The function.</param>
        /// <param name="action">The action.</param>
        /// <returns>IWebJobsBuilder.</returns>
        /// <exception cref="Exception">Something bad happened...</exception>
        public static IWebJobsBuilder LaunchWith(
            this IWebJobsBuilder builder,
            [NotNull] object startupInstance,
            [NotNull] Func<IWebJobsBuilder, object, IRocketFunctionHostBuilder> func,
            [NotNull] Action<IRocketFunctionHostBuilder> action
        )
        {
            if (startupInstance == null)
            {
                throw new ArgumentNullException(nameof(startupInstance));
            }

            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var b = func(builder, startupInstance);
            action(b);
            if (!Builders.TryGetValue(builder, out var conventionalBuilder))
            {
                throw new Exception("Something bad happened...");
            }

            conventionalBuilder.Compose();
            return builder;
        }

        /// <summary>
        /// Launches the with.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="startupInstance">The startup instance.</param>
        /// <param name="func">The function.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="action">The action.</param>
        /// <returns>IWebJobsBuilder.</returns>
        /// <exception cref="Exception">Something bad happened...</exception>
        public static IWebJobsBuilder LaunchWith(
            this IWebJobsBuilder builder,
            [NotNull] object startupInstance,
            [NotNull] Func<IWebJobsBuilder, object, IRocketFunctionHostBuilder> func,
            [NotNull] IHostEnvironment environment,
            [NotNull] Action<IRocketFunctionHostBuilder> action
        )
        {
            if (startupInstance == null)
            {
                throw new ArgumentNullException(nameof(startupInstance));
            }

            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var b = func(builder, startupInstance);
            b.UseEnvironment(environment);
            action(b);
            if (!Builders.TryGetValue(builder, out var conventionalBuilder))
            {
                throw new Exception("Something bad happened...");
            }

            conventionalBuilder.Compose();
            return builder;
        }

        /// <summary>
        /// Uses the scanner.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="scanner">The scanner.</param>
        /// <returns>IRocketFunctionHostBuilder.</returns>
        public static IRocketFunctionHostBuilder UseScanner(
            [NotNull] this IRocketFunctionHostBuilder builder,
            [NotNull] IConventionScanner scanner
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (scanner == null)
            {
                throw new ArgumentNullException(nameof(scanner));
            }

            return Swap(builder, GetOrCreateBuilder(builder, builder.FunctionsAssembly, null).With(scanner));
        }

        /// <summary>
        /// Uses the functions assembly.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assembly">The assembly.</param>
        /// <returns>IRocketFunctionHostBuilder.</returns>
        public static IRocketFunctionHostBuilder UseFunctionsAssembly(
            [NotNull] this IRocketFunctionHostBuilder builder,
            [NotNull] Assembly assembly
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return Swap(builder, GetOrCreateBuilder(builder, builder.FunctionsAssembly, null).With(assembly));
        }

        /// <summary>
        /// Uses the assembly candidate finder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <returns>IRocketFunctionHostBuilder.</returns>
        public static IRocketFunctionHostBuilder UseAssemblyCandidateFinder(
            [NotNull] this IRocketFunctionHostBuilder builder,
            [NotNull] IAssemblyCandidateFinder assemblyCandidateFinder
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (assemblyCandidateFinder == null)
            {
                throw new ArgumentNullException(nameof(assemblyCandidateFinder));
            }

            return Swap(
                builder,
                GetOrCreateBuilder(builder, builder.FunctionsAssembly, null).With(assemblyCandidateFinder)
            );
        }

        /// <summary>
        /// Uses the assembly provider.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <returns>IRocketFunctionHostBuilder.</returns>
        public static IRocketFunctionHostBuilder UseAssemblyProvider(
            [NotNull] this IRocketFunctionHostBuilder builder,
            [NotNull] IAssemblyProvider assemblyProvider
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (assemblyProvider == null)
            {
                throw new ArgumentNullException(nameof(assemblyProvider));
            }

            return Swap(builder, GetOrCreateBuilder(builder, builder.FunctionsAssembly, null).With(assemblyProvider));
        }

        /// <summary>
        /// Uses the diagnostic source.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>IRocketFunctionHostBuilder.</returns>
        public static IRocketFunctionHostBuilder UseDiagnosticSource(
            [NotNull] this IRocketFunctionHostBuilder builder,
            [NotNull] DiagnosticSource diagnosticSource
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (diagnosticSource == null)
            {
                throw new ArgumentNullException(nameof(diagnosticSource));
            }

            return Swap(builder, GetOrCreateBuilder(builder, builder.FunctionsAssembly, null).With(diagnosticSource));
        }

        /// <summary>
        /// Uses the environment.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="environment">The environment.</param>
        /// <returns>IRocketFunctionHostBuilder.</returns>
        public static IRocketFunctionHostBuilder UseEnvironment(
            [NotNull] this IRocketFunctionHostBuilder builder,
            [NotNull] IHostEnvironment environment
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            return Swap(builder, GetOrCreateBuilder(builder, builder.FunctionsAssembly, null).With(environment));
        }

        /// <summary>
        /// Uses the dependency context.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="dependencyContext">The dependency context.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>IRocketFunctionHostBuilder.</returns>
        public static IRocketFunctionHostBuilder UseDependencyContext(
            [NotNull] this IRocketFunctionHostBuilder builder,
            [NotNull] DependencyContext dependencyContext,
            DiagnosticSource? diagnosticSource = null
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (dependencyContext == null)
            {
                throw new ArgumentNullException(nameof(dependencyContext));
            }

            return RocketBooster.ForDependencyContext(dependencyContext, diagnosticSource)(
                builder.Builder,
                builder.FunctionsAssembly
            );
        }

        /// <summary>
        /// Uses the application domain.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="appDomain">The application domain.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>IRocketFunctionHostBuilder.</returns>
        public static IRocketFunctionHostBuilder UseAppDomain(
            [NotNull] this IRocketFunctionHostBuilder builder,
            [NotNull] AppDomain appDomain,
            DiagnosticSource? diagnosticSource = null
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (appDomain == null)
            {
                throw new ArgumentNullException(nameof(appDomain));
            }

            return RocketBooster.ForAppDomain(appDomain, diagnosticSource)(builder.Builder, builder.FunctionsAssembly);
        }

        /// <summary>
        /// Uses the assemblies.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <returns>IRocketFunctionHostBuilder.</returns>
        public static IRocketFunctionHostBuilder UseAssemblies(
            [NotNull] this IRocketFunctionHostBuilder builder,
            [NotNull] IEnumerable<Assembly> assemblies,
            DiagnosticSource? diagnosticSource = null
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            return RocketBooster.ForAssemblies(assemblies, diagnosticSource)(
                builder.Builder,
                builder.FunctionsAssembly
            );
        }

        /// <summary>
        /// Uses the diagnostic logging.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="action">The action.</param>
        /// <returns>IRocketHostBuilder.</returns>
        public static IRocketFunctionHostBuilder UseDiagnosticLogging(
            [NotNull] this IRocketFunctionHostBuilder builder,
            [NotNull] Action<ILoggingBuilder> action
        )
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            ( builder.DiagnosticSource is DiagnosticListener listener
                ? listener
                : new DiagnosticListener("DiagnosticLogger") ).SubscribeWithAdapter(
                new DiagnosticListenerLoggingAdapter(
                    new ServiceCollection()
                       .AddLogging(action)
                       .BuildServiceProvider()
                       .GetRequiredService<ILoggerFactory>()
                       .CreateLogger("DiagnosticLogger")
                )
            );
            return builder;
        }

        /// <summary>
        /// Gets the or create builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="startupInstance">The startup instance.</param>
        /// <param name="environment">The environment.</param>
        /// <returns>RocketFunctionHostBuilder.</returns>
        internal static RocketFunctionHostBuilder GetOrCreateBuilder(
            IRocketFunctionHostBuilder builder,
            object startupInstance,
            IHostEnvironment? environment
        ) => GetOrCreateBuilder(builder.Builder, startupInstance, environment);

        /// <summary>
        /// Gets the or create builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="startupInstance">The startup instance.</param>
        /// <param name="environment">The environment.</param>
        /// <returns>RocketFunctionHostBuilder.</returns>
        internal static RocketFunctionHostBuilder GetOrCreateBuilder(
            IWebJobsBuilder builder,
            object startupInstance,
            IHostEnvironment? environment
        )
        {
            if (!Builders.TryGetValue(builder, out var conventionalBuilder))
            {
                var diagnosticSource = new DiagnosticListener("Rocket.Surgery.Hosting");
                var functionsAssembly = startupInstance.GetType().Assembly;

                var location = Path.GetDirectoryName(functionsAssembly.Location);
                DependencyContext? dependencyContext = null;
                while (!string.IsNullOrEmpty(location))
                {
                    var depsFilePath = Path.Combine(location, functionsAssembly.GetName().Name + ".deps.json");
                    if (File.Exists(depsFilePath))
                    {
                        using var stream = File.Open(depsFilePath, FileMode.Open, FileAccess.Read);
                        using var reader = new DependencyContextJsonReader();
                        dependencyContext = reader.Read(stream);
                        break;
                    }

                    location = Path.GetDirectoryName(location);
                }

                var logger = new DiagnosticLogger(diagnosticSource);
                var assemblyCandidateFinder = new DependencyContextAssemblyCandidateFinder(dependencyContext!, logger);
                var assemblyProvider = new DependencyContextAssemblyProvider(dependencyContext!, logger);
                var properties = new ServiceProviderDictionary();
                properties.Set<ILogger>(logger);
                properties.Set(HostType.Live);
                var scanner = new SimpleConventionScanner(assemblyCandidateFinder, properties, logger);
                conventionalBuilder = new RocketFunctionHostBuilder(
                    builder,
                    functionsAssembly,
                    startupInstance,
                    environment!,
                    scanner,
                    assemblyCandidateFinder,
                    assemblyProvider,
                    diagnosticSource,
                    properties
                );
                Builders.Add(builder, conventionalBuilder);
            }

            return conventionalBuilder;
        }

        /// <summary>
        /// Swaps the specified builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="newRocketFunctionBuilder">The new rocket function builder.</param>
        /// <returns>RocketFunctionHostBuilder.</returns>
        internal static RocketFunctionHostBuilder Swap(
            IRocketFunctionHostBuilder builder,
            RocketFunctionHostBuilder newRocketFunctionBuilder
        )
        {
            Builders.Remove(builder.Builder);
            Builders.Add(builder.Builder, newRocketFunctionBuilder);
            return newRocketFunctionBuilder;
        }

        private static readonly ConditionalWeakTable<IWebJobsBuilder, RocketFunctionHostBuilder> Builders =
            new ConditionalWeakTable<IWebJobsBuilder, RocketFunctionHostBuilder>();
    }
}