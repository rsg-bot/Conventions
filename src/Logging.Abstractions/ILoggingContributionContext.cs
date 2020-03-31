using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.Logging
{
    /// <summary>
    /// ILoggingConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// Implements the <see cref="Microsoft.Extensions.Logging.ILoggingBuilder" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    /// <seealso cref="Microsoft.Extensions.Logging.ILoggingBuilder" />
    public interface ILoggingConventionContext : IConventionContext, Microsoft.Extensions.Logging.ILoggingBuilder
    {
        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        [NotNull] IAssemblyProvider AssemblyProvider { get; }

        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        [NotNull] IAssemblyCandidateFinder AssemblyCandidateFinder { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        [NotNull] IConfiguration Configuration { get; }

        /// <summary>
        /// The environment that this convention is running
        /// Based on IHostEnvironment / IHostingEnvironment
        /// </summary>
        /// <value>The environment.</value>
        [NotNull] IHostEnvironment Environment { get; }
    }
}