using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;
using Rocket.Surgery.Extensions.DependencyInjection.Internals;

namespace Rocket.Surgery.Extensions.DependencyInjection
{
    /// <summary>
    /// ServicesBuilder.
    /// Implements the <see cref="ConventionBuilder{IServicesBuilder, IServiceConvention, ServiceConventionDelegate}" />
    /// Implements the <see cref="IServicesBuilder" />
    /// Implements the <see cref="IServiceConvention" />
    /// Implements the <see cref="IServiceConventionContext" />
    /// Implements the <see cref="ServiceConventionDelegate" />
    /// </summary>
    /// <seealso cref="ConventionBuilder{IServicesBuilder, IServiceConvention, ServiceConventionDelegate}" />
    /// <seealso cref="IServicesBuilder" />
    /// <seealso cref="IServiceConvention" />
    /// <seealso cref="IServiceConventionContext" />
    /// <seealso cref="ServiceConventionDelegate" />
    public class ServicesBuilder : ConventionBuilder<IServicesBuilder, IServiceConvention, ServiceConventionDelegate>,
                                   IServicesBuilder
    {
        private readonly ServiceProviderObservable _onBuild;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesBuilder" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="properties">The properties.</param>
        /// <exception cref="ArgumentNullException">
        /// environment
        /// or
        /// diagnosticSource
        /// or
        /// configuration
        /// or
        /// services
        /// </exception>
        public ServicesBuilder(
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment,
            ILogger diagnosticSource,
            IDictionary<object, object?> properties
        )
            : base(scanner, assemblyProvider, assemblyCandidateFinder, properties)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Services = services ?? throw new ArgumentNullException(nameof(services));
            Logger = diagnosticSource ?? throw new ArgumentNullException(nameof(diagnosticSource));
            _onBuild = new ServiceProviderObservable(Logger);
            ServiceProviderOptions = new ServiceProviderOptions
            {
                ValidateScopes = environment.IsDevelopment()
            };
        }

        /// <summary>
        /// Gets the service provider options.
        /// </summary>
        /// <value>The service provider options.</value>
        public ServiceProviderOptions ServiceProviderOptions { get; }

        /// <summary>
        /// Builds the root container, and returns the lifetime scopes for the application and system containers
        /// </summary>
        /// <returns>IServiceProvider.</returns>
        public IServiceProvider Build()
        {
            Composer.Register(Scanner, this, typeof(IServiceConvention), typeof(ServiceConventionDelegate));

            var result = Services.BuildServiceProvider(ServiceProviderOptions);
            _onBuild.Send(result);
            return result;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the services.
        /// </summary>
        /// <value>The services.</value>
        public IServiceCollection Services { get; }

        /// <summary>
        /// A logger that is configured to work with each convention item
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets the on build.
        /// </summary>
        /// <value>The on build.</value>
        public IObservable<IServiceProvider> OnBuild => _onBuild;

        /// <summary>
        /// The environment that this convention is running
        /// Based on IHostEnvironment / IHostingEnvironment
        /// </summary>
        /// <value>The environment.</value>
        public IHostEnvironment Environment { get; }
    }
}