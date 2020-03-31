using System;
using JetBrains.Annotations;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Ensures the convention runs after the given <see cref="IConvention" />
    /// </summary>
    /// <seealso cref="Attribute" />
    public interface IConventionDependency
    {
        /// <summary>
        /// The <see cref="IConvention" /> type to link to
        /// </summary>
        [NotNull]
        Type Type { get; }


        /// <summary>
        /// The <see cref="DependencyDirection" /> direction of this relationship
        /// </summary>
        DependencyDirection Direction { get; }
    }
}