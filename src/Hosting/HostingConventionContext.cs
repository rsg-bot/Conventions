using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Hosting
{
    internal class HostingConventionContext : ConventionContext, IHostingConventionContext
    {
        private readonly IConventionHostBuilder _builder;
        private readonly IHostBuilder _hostBuilder;

        public HostingConventionContext(
            IConventionHostBuilder builder,
            IHostBuilder hostBuilder,
            ILogger logger
        ) : base(logger, builder.ServiceProperties)
        {
            _builder = builder;
            _hostBuilder = hostBuilder;
        }

        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <value>The builder.</value>
        public IHostBuilder Builder => _hostBuilder;

        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        public IAssemblyProvider AssemblyProvider => _builder.AssemblyProvider;

        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        public IAssemblyCandidateFinder AssemblyCandidateFinder => _builder.AssemblyCandidateFinder;

        public IConventionScanner Scanner => _builder.Scanner;

        public IServiceProviderDictionary ServiceProperties => _builder.ServiceProperties;

        public DiagnosticSource DiagnosticSource => _builder.DiagnosticSource;

        public IConventionHostBuilder AppendConvention(IEnumerable<IConvention> conventions)
            => _builder.AppendConvention(conventions);

        public IConventionHostBuilder AppendConvention(params IConvention[] conventions)
            => _builder.AppendConvention(conventions);

        public IConventionHostBuilder AppendConvention(IEnumerable<Type> conventions)
            => _builder.AppendConvention(conventions);

        public IConventionHostBuilder AppendConvention(params Type[] conventions)
            => _builder.AppendConvention(conventions);

        public IConventionHostBuilder AppendConvention<T>()
            where T : IConvention => _builder.AppendConvention<T>();

        public IConventionHostBuilder AppendDelegate(IEnumerable<Delegate> delegates)
            => _builder.AppendDelegate(delegates);

        public IConventionHostBuilder AppendDelegate(params Delegate[] delegates) => _builder.AppendDelegate(delegates);

        public IConventionHostBuilder PrependConvention(IEnumerable<IConvention> conventions)
            => _builder.PrependConvention(conventions);

        public IConventionHostBuilder PrependConvention(params IConvention[] conventions)
            => _builder.PrependConvention(conventions);

        public IConventionHostBuilder PrependConvention(IEnumerable<Type> conventions)
            => _builder.PrependConvention(conventions);

        public IConventionHostBuilder PrependConvention(params Type[] conventions)
            => _builder.PrependConvention(conventions);

        public IConventionHostBuilder PrependConvention<T>()
            where T : IConvention => _builder.PrependConvention<T>();

        public IConventionHostBuilder PrependDelegate(IEnumerable<Delegate> delegates)
            => _builder.PrependDelegate(delegates);

        public IConventionHostBuilder PrependDelegate(params Delegate[] delegates)
            => _builder.PrependDelegate(delegates);
    }
}