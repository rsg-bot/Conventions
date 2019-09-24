using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DependencyInjection;
using Rocket.Surgery.Extensions.Logging;

namespace Rocket.Surgery.Conventions.TestHost
{
    // /// <summary>
    // /// Class ConventionTestHostBuilder.
    // /// Implements the <see cref="ConventionHostBuilder{ConventionTestHostBuilder}" />
    // /// </summary>
    // /// <seealso cref="ConventionHostBuilder{ConventionTestHostBuilder}" />
    // public class ConventionTestHost : ConventionHostBuilder<ConventionTestHost>
    // {
    //     private readonly ILogger _logger;

    //     /// <summary>
    //     /// Initializes a new instance of the <see cref="ConventionTestHost"/> class.
    //     /// </summary>
    //     /// <param name="scanner">The scanner.</param>
    //     /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
    //     /// <param name="assemblyProvider">The assembly provider.</param>
    //     /// <param name="diagnosticSource">The diagnostic source.</param>
    //     /// <param name="serviceProperties">The service properties.</param>
    //     internal ConventionTestHost(IConventionScanner scanner, IAssemblyCandidateFinder assemblyCandidateFinder, IAssemblyProvider assemblyProvider, DiagnosticSource diagnosticSource, IServiceProviderDictionary serviceProperties) : base(scanner, assemblyCandidateFinder, assemblyProvider, diagnosticSource, serviceProperties)
    //     {
    //         _logger = new DiagnosticLogger(diagnosticSource);
    //     }

    //     /// <summary>
    //     /// Use the <see cref="BasicConventionScanner" /> to not automatically load conventions from attributes.
    //     /// </summary>
    //     /// <returns></returns>
    //     public ConventionTestHost DoNotIncludeConventionAttributes()
    //     {
    //         return new ConventionTestHost(new BasicConventionScanner(ServiceProperties), AssemblyCandidateFinder, AssemblyProvider, DiagnosticSource, ServiceProperties);
    //     }

    //     /// <summary>
    //     /// Use the <see cref="SimpleConventionScanner" /> to automatically load conventions from attributes.\
    //     ///
    //     /// This is the default
    //     /// </summary>
    //     /// <returns></returns>
    //     public ConventionTestHost IncludeConventionAttributes()
    //     {
    //         return new ConventionTestHost(new SimpleConventionScanner(AssemblyCandidateFinder, ServiceProperties, _logger), AssemblyCandidateFinder, AssemblyProvider, DiagnosticSource, ServiceProperties);
    //     }

    //     public (IConfigurationRoot Configuration, IServiceProvider Container) Build(IRocketEnvironment environment)
    //     {
    //         var configurationBuilder = new ConfigurationBuilder();
    //         var cb = new Rocket.Surgery.Extensions.Configuration.ConfigurationBuilder(
    //             Scanner,
    //             environment,
    //             new ConfigurationRoot(new List<IConfigurationProvider>()),
    //             configurationBuilder,
    //             _logger,
    //             ServiceProperties);

    //         cb.Build();
    //         var configuration = configurationBuilder.Build();

    //         var servicesBuilder = new ServicesBuilder(
    //             Scanner,
    //             AssemblyProvider,
    //             AssemblyCandidateFinder,
    //             new ServiceCollection(),
    //             configuration,
    //             environment,
    //             _logger,
    //             ServiceProperties
    //         );

    //         servicesBuilder.Services.AddLogging(builder =>
    //         {
    //             var loggingBuilder = new LoggingBuilder(
    //                 Scanner,
    //                 AssemblyProvider,
    //                 AssemblyCandidateFinder,
    //                 builder.Services,
    //                 environment,
    //                 configuration,
    //                 _logger,
    //                 ServiceProperties
    //             );
    //             loggingBuilder.Build();
    //         });

    //         var container = servicesBuilder.Build();

    //         return (configuration, container);
    //     }
    // }
}