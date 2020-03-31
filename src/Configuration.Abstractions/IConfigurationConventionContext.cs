using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Rocket.Surgery.Conventions;

// ReSharper disable PossibleInterfaceMemberAmbiguity

namespace Rocket.Surgery.Extensions.Configuration
{
    /// <summary>
    /// IConfigurationConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// Implements the <see cref="Microsoft.Extensions.Configuration.IConfigurationBuilder" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    /// <seealso cref="Microsoft.Extensions.Configuration.IConfigurationBuilder" />
    public interface IConfigConventionContext : IConventionContext,
                                                       Microsoft.Extensions.Configuration.IConfigurationBuilder
    {
        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        [NotNull] new IDictionary<object, object?> Properties { get; }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        [NotNull] IConfiguration Configuration { get; }
    }
}