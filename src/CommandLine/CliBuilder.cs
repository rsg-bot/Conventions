using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.CommandLine.Rendering;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;
using Rocket.Surgery.Conventions.Scanners;

#pragma warning disable CA1001

namespace Rocket.Surgery.Extensions.CommandLine
{
    /// <summary>
    /// Logging Builder
    /// Implements the <see cref="ICliBuilder" />
    /// Implements the <see cref="ICliConventionContext" />
    /// </summary>
    /// <seealso cref="ICliBuilder" />
    /// <seealso cref="ICliConventionContext" />
    public class CliBuilder :
        ConventionBuilder<ICliBuilder, ICliConvention, CliConventionDelegate>,
        ICliBuilder,
        ICliConventionContext
    {
        private readonly RootCommand _command;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineBuilder" /> class.
        /// </summary>
        /// <param name="scanner">The scanner.</param>
        /// <param name="assemblyProvider">The assembly provider.</param>
        /// <param name="assemblyCandidateFinder">The assembly candidate finder.</param>
        /// <param name="diagnosticSource">The diagnostic source.</param>
        /// <param name="properties">The properties.</param>
        /// <exception cref="ArgumentNullException">diagnosticSource</exception>
        public CliBuilder(
            IConventionScanner scanner,
            IAssemblyProvider assemblyProvider,
            IAssemblyCandidateFinder assemblyCandidateFinder,
            ILogger diagnosticSource,
            IDictionary<object, object?> properties
        ) : base(scanner, assemblyProvider, assemblyCandidateFinder, properties)
        {
            _command = new RootCommand();
            Builder = new CommandLineBuilder(_command)
               .UseDefaults()
               .UseAnsiTerminalWhenAvailable();
            Logger = diagnosticSource ?? throw new ArgumentNullException(nameof(diagnosticSource));
        }

        /// <summary>
        /// Builds the specified entry assembly.
        /// </summary>
        /// <param name="entryAssembly">The entry assembly.</param>
        /// <returns>ICommandLine.</returns>
        public Parser Build(Assembly? entryAssembly = null)
        {
            if (entryAssembly is null)
            {
                entryAssembly = Assembly.GetEntryAssembly()!;
            }

            Builder
               .EnableDirectives()
               .EnablePosixBundling();

            Composer.Register(Scanner, this, typeof(ICliConvention), typeof(CliConventionDelegate));

            var verbose = new Option<bool>(new[] { "-v", "--verbose" }, "Enable verbose logging");
            var trace = new Option<bool>(new[] { "-t", "--trace" }, "Enable trace logging");
            var debug = new Option<bool>(new[] { "-d", "--debug" }, "Enable debug logging");
            var logLevel = new Option<LogLevel>(new[] { "-ll", "--log-level" }, "Set the log level");
            
            _command.AddGlobalOption(verbose);
            _command.AddGlobalOption(trace);
            _command.AddGlobalOption(debug);
            _command.AddGlobalOption(logLevel);

            var parser =  Builder.Build();
            return parser;
        }

        /// <summary>
        /// A logger that is configured to work with each convention item
        /// </summary>
        /// <value>The logger.</value>
        public ILogger Logger { get; }
        
        /// <summary>
        /// The commandline builder
        /// </summary>
        public CommandLineBuilder Builder { get; }
    }
}