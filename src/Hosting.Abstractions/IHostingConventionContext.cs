using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Hosting
{
    /// <summary>
    /// IHostingConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    public interface IHostingConventionContext : IConventionContext
    {
        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <value>The builder.</value>
        IHostBuilder Builder { get; }
    }
}