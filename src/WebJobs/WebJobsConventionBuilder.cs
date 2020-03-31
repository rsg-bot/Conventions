using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Extensions.WebJobs
{
    /// <summary>
    /// WebJobsConventionBuilder.
    /// Implements the <see cref="ConventionBuilder{TBuilder,TConvention,TDelegate}" />
    /// Implements the <see cref="IWebJobsConvention" />
    /// Implements the <see cref="IWebJobsConventionContext" />
    /// Implements the <see cref="WebJobsConventionDelegate" />
    /// </summary>
    /// <seealso cref="ConventionBuilder{IWebJobsConventionBuilder, IWebJobsConvention, WebJobsConventionDelegate}" />
    /// <seealso cref="IWebJobsConvention" />
    /// <seealso cref="IWebJobsConventionContext" />
    /// <seealso cref="WebJobsConventionDelegate" />
    public class WebJobsConventionBuilder :
        ConventionBuilder<WebJobsConventionBuilder, IWebJobsConvention, WebJobsConventionDelegate>,
        IWebJobsConventionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebJobsConventionBuilder" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="webJobsBuilder">The web jobs builder.</param>
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
        /// webJobsBuilder
        /// or
        /// webJobsBuilder
        /// </exception>
        public WebJobsConventionBuilder(
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            IWebJobsBuilder webJobsBuilder,
            IConfiguration configuration,
            IHostEnvironment environment,
            ILogger diagnosticSource,
            IDictionary<object, object?> properties
        )
            : base(scanner, assemblyProvider, assemblyCandidateFinder, properties)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            WebJobsBuilder = webJobsBuilder ?? throw new ArgumentNullException(nameof(webJobsBuilder));
            Services = webJobsBuilder.Services ?? throw new ArgumentNullException(nameof(webJobsBuilder));
            Logger = diagnosticSource ?? throw new ArgumentNullException(nameof(diagnosticSource));
        }

        /// <summary>
        /// Gets the web jobs builder.
        /// </summary>
        /// <value>The web jobs builder.</value>
        public IWebJobsBuilder WebJobsBuilder { get; }

        /// <summary>
        /// Calls all conventions and loads them into the webJobsBuilder
        /// </summary>
        public void Build() => Composer.Register(
            Scanner,
            this,
            typeof(IWebJobsConvention),
            typeof(WebJobsConventionDelegate)
        );

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
        /// The environment that this convention is running
        /// Based on IHostEnvironment / IHostingEnvironment
        /// </summary>
        /// <value>The environment.</value>
        public IHostEnvironment Environment { get; }
    }
}