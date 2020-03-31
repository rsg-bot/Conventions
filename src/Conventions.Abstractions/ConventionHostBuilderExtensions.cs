using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Hosting;

#pragma warning disable CS8601 // Possible null reference assignment.

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Base convention extensions
    /// </summary>
    public static class ConventionHostBuilderExtensions
    {
        /// <summary>
        /// Gets the convention host builder or creates one of it's missing.
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns></returns>
        public static IConventionHostBuilder GetConventions(this IHostBuilder hostBuilder)
        {
            if (hostBuilder.Properties.TryGetValue(
                typeof(IConventionHostBuilder),
                out var value
            ) && value is IConventionHostBuilder conventionHostBuilder)
            {
                return conventionHostBuilder;
            }

            conventionHostBuilder = new UninitializedConventionHostBuilder(hostBuilder.Properties);
            hostBuilder.Properties.Add(typeof(IConventionHostBuilder), conventionHostBuilder);
            return conventionHostBuilder;
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context</param>
        /// <returns>T.</returns>
        [NotNull]
        public static T Get<T>([NotNull] this IConventionHostBuilder context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return (T)context.ServiceProperties[typeof(T)];
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context">The context</param>
        /// <param name="key">The key where the value is saved</param>
        /// <returns>T.</returns>
        [NotNull]
        public static T Get<T>([NotNull] this IConventionHostBuilder context, string key)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return (T)context.ServiceProperties[key];
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder">The builder</param>
        /// <param name="factory">The factory method in the event the type is not found</param>
        /// <returns>T.</returns>
        [NotNull]
        public static T GetOrAdd<T>([NotNull] this IConventionHostBuilder builder, [NotNull] Func<T> factory)
            where T : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (builder.ServiceProperties[typeof(T)] is T value)
            {
                return value;
            }

            value = factory();
            builder.Set(value);

            return value;
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder">The builder</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="factory">The factory method in the event the type is not found</param>
        /// <returns>T.</returns>
        [NotNull]
        public static T GetOrAdd<T>(
            [NotNull] this IConventionHostBuilder builder,
            string key,
            [NotNull] Func<T> factory
        )
            where T : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (!( builder.ServiceProperties[key] is T value ))
            {
                value = factory();
                builder.Set(value);
            }

            return value;
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="context">The context</param>
        /// <param name="value">The value to save</param>
        public static void Set<T>([NotNull] this IConventionHostBuilder context, T value)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.ServiceProperties[typeof(T)] = value;
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="context">The context</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="value">The value to save</param>
        public static void Set<T>([NotNull] this IConventionHostBuilder context, string key, T value)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.ServiceProperties[key] = value;
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <returns>T.</returns>
        [NotNull]
        public static T Get<T>([NotNull] this IServiceProviderDictionary serviceProviderDictionary)
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            return (T)serviceProviderDictionary[typeof(T)];
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="key">The key where the value is saved</param>
        /// <returns>T.</returns>
        [NotNull]
        public static T Get<T>([NotNull] this IServiceProviderDictionary serviceProviderDictionary, string key)
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            return (T)serviceProviderDictionary[key];
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="factory">The factory method in the event the type is not found</param>
        /// <returns>T.</returns>
        [NotNull]
        public static T GetOrAdd<T>(
            [NotNull] this IServiceProviderDictionary serviceProviderDictionary,
            [NotNull] Func<T> factory
        )
            where T : class
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (!( serviceProviderDictionary[typeof(T)] is T value ))
            {
                value = factory();
                serviceProviderDictionary.Set(value);
            }

            return value;
        }

        /// <summary>
        /// Get a value by key from the context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="factory">The factory method in the event the type is not found</param>
        /// <returns>T.</returns>
        [NotNull]
        public static T GetOrAdd<T>(
            [NotNull] this IServiceProviderDictionary serviceProviderDictionary,
            string key,
            [NotNull] Func<T> factory
        )
            where T : class
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (!( serviceProviderDictionary[key] is T value ))
            {
                value = factory();
                serviceProviderDictionary.Set(value);
            }

            return value;
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="value">The value to save</param>
        public static void Set<T>([NotNull] this IServiceProviderDictionary serviceProviderDictionary, T value)
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            serviceProviderDictionary[typeof(T)] = value;
        }

        /// <summary>
        /// Get a value by type from the context
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="serviceProviderDictionary">The properties</param>
        /// <param name="key">The key where the value is saved</param>
        /// <param name="value">The value to save</param>
        public static void Set<T>(
            [NotNull] this IServiceProviderDictionary serviceProviderDictionary,
            string key,
            T value
        )
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            serviceProviderDictionary[key] = value;
        }

        /// <summary>
        /// Check if this is a test host (to allow conventions to behave differently during unit tests)
        /// </summary>
        /// <param name="context">The context</param>
        public static bool IsUnitTestHost([NotNull] this IConventionHostBuilder context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.ServiceProperties.IsUnitTestHost();
        }

        /// <summary>
        /// Check if this is a test host (to allow conventions to behave differently during unit tests)
        /// </summary>
        /// <param name="serviceProviderDictionary">The properties</param>
        public static bool IsUnitTestHost([NotNull] this IServiceProviderDictionary serviceProviderDictionary)
        {
            if (serviceProviderDictionary == null)
            {
                throw new ArgumentNullException(nameof(serviceProviderDictionary));
            }

            return serviceProviderDictionary.GetHostType() == HostType.UnitTestHost;
        }

        /// <summary>
        /// Check if this is a test host (to allow conventions to behave differently during unit tests)
        /// </summary>
        /// <param name="context">The context</param>
        internal static HostType GetHostType(this IConventionHostBuilder context)
            => context.ServiceProperties.TryGetValue(typeof(HostType), out var hostType)
                ? (HostType)hostType!
                : HostType.Undefined;

        /// <summary>
        /// Check if this is a test host (to allow conventions to behave differently during unit tests)
        /// </summary>
        /// <param name="serviceProviderDictionary">The properties</param>
        internal static HostType GetHostType(this IServiceProviderDictionary serviceProviderDictionary)
            => serviceProviderDictionary.TryGetValue(typeof(HostType), out var hostType)
                ? (HostType)hostType!
                : HostType.Undefined;
    }
}