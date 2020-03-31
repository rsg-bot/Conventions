using JetBrains.Annotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.WebJobs
{
    /// <summary>
    /// IWebJobsConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// Implements the <see cref="IWebJobsBuilder" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    /// <seealso cref="IWebJobsBuilder" />
    public interface IWebJobsConventionContext : IConventionContext, IWebJobsBuilder
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        [NotNull] IConfiguration Configuration { get; }

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
        /// The environment that this convention is running
        /// Based on IHostEnvironment / IHostingEnvironment
        /// </summary>
        /// <value>The environment.</value>
        [NotNull] IHostEnvironment Environment { get; }
    }
}