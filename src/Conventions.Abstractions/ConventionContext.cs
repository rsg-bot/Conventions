using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Base convention context that allos for stashing items in index keys
    /// Implements the <see cref="IConventionContext" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    public abstract class ConventionContext : IConventionContext
    {
        /// <summary>
        /// Creates a base context
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="properties"></param>
        protected ConventionContext(ILogger logger, IDictionary<object, object?> properties)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Properties = properties ?? new Dictionary<object, object?>();
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.Object.</returns>
        public virtual object? this[object item]
        {
            get => Properties.TryGetValue(item, out var value) ? value : null;
            set => Properties[item] = value;
        }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <value>The properties.</value>
        public IDictionary<object, object?> Properties { get; }

        /// <summary>
        /// A logger that is configured to work with each convention item
        /// </summary>
        /// <value>The logger.</value>
        /// <inheritdoc />
        public ILogger Logger { get; }
    }
}