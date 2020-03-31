using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// An attribute that ensures the convention runs after the given <see cref="IConvention" />
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class AfterConventionAttribute : Attribute, IConventionDependency
    {
        /// <summary>
        /// The type to be used with the convention type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="NotSupportedException">Type must inherit from " + nameof(IConvention)</exception>
        public AfterConventionAttribute([NotNull] Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(IConvention).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                throw new NotSupportedException("Type must inherit from " + nameof(IConvention));
            }

            Type = type;
        }

        DependencyDirection IConventionDependency.Direction => DependencyDirection.DependsOn;

        /// <summary>
        /// The convention type
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; }
    }
}