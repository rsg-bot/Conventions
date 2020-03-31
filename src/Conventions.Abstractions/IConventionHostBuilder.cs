using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    class UninitializedConventionHostBuilder : IConventionHostBuilder
    {
        public UninitializedConventionHostBuilder(IDictionary<object, object> properties)
        {
            Scanner = new UninitializedConventionScanner();
            AssemblyCandidateFinder = new UninitializedAssemblyCandidateFinder();
            ServiceProperties = new ServiceProviderDictionary(properties);
            DiagnosticSource = null;
            AssemblyProvider = new UninitializedAssemblyProvider();

        }
        public UninitializedConventionScanner Scanner { get; }
        IConventionScanner IConventionHostBuilder.Scanner => Scanner;
        public IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        public IServiceProviderDictionary ServiceProperties { get; }
        public IAssemblyProvider AssemblyProvider { get; }
        public DiagnosticSource? DiagnosticSource { get; }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder AppendConvention(params IConvention[] conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder AppendConvention(IEnumerable<IConvention> conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder AppendConvention(params Type[] conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder AppendConvention(IEnumerable<Type> conventions)
        {
            Scanner.AppendConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder AppendConvention<T>()
            where T : IConvention
        {
            Scanner.AppendConvention<T>();
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder PrependConvention(params IConvention[] conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder PrependConvention(IEnumerable<IConvention> conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder PrependConvention(params Type[] conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder PrependConvention(IEnumerable<Type> conventions)
        {
            Scanner.PrependConvention(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder PrependConvention<T>()
            where T : IConvention
        {
            Scanner.PrependConvention<T>();
            return this;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder PrependDelegate(params Delegate[] delegates)
        {
            Scanner.PrependDelegate(delegates);
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder PrependDelegate(IEnumerable<Delegate> delegates)
        {
            Scanner.PrependDelegate(delegates);
            return this;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder AppendDelegate(params Delegate[] delegates)
        {
            Scanner.AppendDelegate(delegates);
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        public IConventionHostBuilder AppendDelegate(IEnumerable<Delegate> delegates)
        {
            Scanner.AppendDelegate(delegates);
            return this;
        }
    }

    class UninitializedConventionScanner : IConventionScanner
    {
        internal readonly List<object> _prependedConventions = new List<object>();
        internal  readonly List<object> _appendedConventions = new List<object>();
        internal  readonly List<Type> _exceptConventions = new List<Type>();
        internal  readonly List<Assembly> _exceptAssemblyConventions = new List<Assembly>();

        public IConventionProvider BuildProvider() => throw new NotImplementedException();
        
        
        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention(IEnumerable<IConvention> conventions)
        {
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention(IEnumerable<Type> conventions)
        {
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention(params IConvention[] conventions)
        {
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention(params Type[] conventions)
        {
            _appendedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendConvention<T>()
            where T : IConvention
        {
            _appendedConventions.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention(IEnumerable<IConvention> conventions)
        {
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention(IEnumerable<Type> conventions)
        {
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention(params IConvention[] conventions)
        {
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention(params Type[] conventions)
        {
            _prependedConventions.AddRange(conventions);
            return this;
        }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependConvention<T>()
            where T : IConvention
        {
            _prependedConventions.Add(typeof(T));
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendDelegate(IEnumerable<Delegate> delegates)
        {
            _appendedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner AppendDelegate(params Delegate[] delegates)
        {
            _appendedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependDelegate(IEnumerable<Delegate> delegates)
        {
            _prependedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner PrependDelegate(params Delegate[] delegates)
        {
            _prependedConventions.AddRange(delegates);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="types">The convention types to exclude.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner ExceptConvention(IEnumerable<Type> types)
        {
            _exceptConventions.AddRange(types);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="types">The additional types to exclude.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner ExceptConvention(params Type[] types)
        {
            _exceptConventions.AddRange(types);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="assemblies">The convention types to exclude.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner ExceptConvention(IEnumerable<Assembly> assemblies)
        {
            _exceptAssemblyConventions.AddRange(assemblies);
            return this;
        }

        /// <summary>
        /// Adds an exception to the scanner to exclude a specific type
        /// </summary>
        /// <param name="assemblies">The additional types to exclude.</param>
        /// <returns>IConventionScanner.</returns>
        /// <inheritdoc />
        public IConventionScanner ExceptConvention(params Assembly[] assemblies)
        {
            _exceptAssemblyConventions.AddRange(assemblies);
            return this;
        }
    }

    class UninitializedAssemblyCandidateFinder : IAssemblyCandidateFinder
    {
        public IEnumerable<Assembly> GetCandidateAssemblies(IEnumerable<string> candidates) => throw new NotImplementedException();
    }

    class UninitializedAssemblyProvider : IAssemblyProvider
    {
        public IEnumerable<Assembly> GetAssemblies() => throw new NotImplementedException();
    }
    
    /// <summary>
    /// IConventionHostBuilder
    /// </summary>
    public interface IConventionHostBuilder
    {
        /// <summary>
        /// Gets the scanner.
        /// </summary>
        /// <value>The scanner.</value>
        [NotNull]
        IConventionScanner Scanner { get; }

        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        [NotNull]
        IAssemblyCandidateFinder AssemblyCandidateFinder { get; }

        /// <summary>
        /// A central location for sharing state between components during the convention building process.
        /// </summary>
        /// <value>The properties.</value>
        [NotNull]
        IServiceProviderDictionary ServiceProperties { get; }

        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        [NotNull]
        IAssemblyProvider AssemblyProvider { get; }

        /// <summary>
        /// Gets the diagnostic source.
        /// </summary>
        /// <value>The diagnostic source.</value>
        DiagnosticSource? DiagnosticSource { get; }

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder AppendConvention([NotNull] IEnumerable<IConvention> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder AppendConvention(params IConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder AppendConvention([NotNull] IEnumerable<Type> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder AppendConvention(params Type[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder AppendConvention<T>()
            where T : IConvention;

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder PrependConvention([NotNull] IEnumerable<IConvention> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder PrependConvention(params IConvention[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder PrependConvention([NotNull] IEnumerable<Type> conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <param name="conventions">The additional conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder PrependConvention(params Type[] conventions);

        /// <summary>
        /// Adds a set of conventions to the scanner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder PrependConvention<T>()
            where T : IConvention;

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder AppendDelegate([NotNull] IEnumerable<Delegate> delegates);


        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder AppendDelegate(params Delegate[] delegates);

        /// <summary>
        /// Adds a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The conventions.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder PrependDelegate([NotNull] IEnumerable<Delegate> delegates);

        /// <summary>
        /// Addes a set of delegates to the scanner
        /// </summary>
        /// <param name="delegates">The additional delegates.</param>
        /// <returns>IConventionHostBuilder.</returns>
        [NotNull]
        IConventionHostBuilder PrependDelegate(params Delegate[] delegates);
    }
}