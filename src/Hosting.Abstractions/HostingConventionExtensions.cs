using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;
using Rocket.Surgery.Hosting;

// ReSharper disable once CheckNamespace
namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Helper method for working with <see cref="IConventionHostBuilder" />
    /// </summary>
    public static class HostingConventionExtensions
    {
        /// <summary>
        /// Configure the hosting delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IHostBuilder ConfigureHosting(
            [NotNull] this IHostBuilder container,
            HostingConventionDelegate @delegate
        )
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.GetConventions().Scanner.AppendDelegate(@delegate);
            return container;
        }
        
        /// <summary>
        /// Configure the hosting delegate to the convention scanner
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="delegate">The delegate.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public static IConventionHostBuilder ConfigureHosting(
            [NotNull] this IConventionHostBuilder container,
            HostingConventionDelegate @delegate
        )
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Scanner.AppendDelegate(@delegate);
            return container;
        }
    }
}