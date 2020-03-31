using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Scanners;
using IMsftConfigurationBuilder = Microsoft.Extensions.Configuration.IConfigurationBuilder;
using MsftConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace Rocket.Surgery.Extensions.Configuration
{
    /// <summary>
    /// Logging Builder
    /// Implements the <see cref="ConventionContainerBuilder{TBuilder,TConvention,TDelegate}" />
    /// Implements the <see cref="IConfigBuilder" />
    /// Implements the <see cref="IConfigConvention" />
    /// Implements the <see cref="IConfigConventionContext" />
    /// Implements the <see cref="ConfigConventionDelegate" />
    /// </summary>
    /// <seealso
    ///     cref="ConventionContainerBuilder{IConfigurationBuilder, IConfigurationConvention, ConfigurationConventionDelegate}" />
    /// <seealso cref="IConfigBuilder" />
    /// <seealso cref="IConfigConvention" />
    /// <seealso cref="IConfigConventionContext" />
    /// <seealso cref="ConfigConventionDelegate" />
    public sealed class ConfigBuilder :
        ConventionContainerBuilder<ConfigBuilder, IConfigConvention, ConfigConventionDelegate>,
        IConfigConventionContext
    {
        private readonly IMsftConfigurationBuilder _builder = new MsftConfigurationBuilder();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigBuilder" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="properties">The properties.</param>
        /// <exception cref="ArgumentNullException">
        /// scanner
        /// or
        /// builder
        /// or
        /// configuration
        /// or
        /// diagnosticSource
        /// </exception>
        public ConfigBuilder(
            IConventionScanner scanner,
            IHostEnvironment environment,
            IConfiguration configuration,
            ILogger diagnosticSource,
            IDictionary<object, object?> properties
        ) : base(scanner, properties)
        {
            Environment = environment;
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Logger = diagnosticSource ?? throw new ArgumentNullException(nameof(diagnosticSource));
        }

        /// <summary>
        /// Gets the environment.
        /// </summary>
        /// <value>The environment.</value>
        public IHostEnvironment Environment { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// A logger that is configured to work with each convention item
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        public IConfigurationRoot Build()
        {
            Composer.Register(Scanner, this, typeof(IConfigConvention), typeof(ConfigConventionDelegate));
            return _builder.Build();
        }

        IMsftConfigurationBuilder IMsftConfigurationBuilder.Add(IConfigurationSource source) => _builder.Add(source);
        IDictionary<string, object> IMsftConfigurationBuilder.Properties => _builder.Properties;
        IList<IConfigurationSource> IMsftConfigurationBuilder.Sources => _builder.Sources;
    }
}