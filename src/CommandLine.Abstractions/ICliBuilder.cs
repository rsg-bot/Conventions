using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Reflection;
using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Extensions.CommandLine
{
    /// <summary>
    /// ILoggingConvention
    /// Implements the <see cref="IConventionBuilder{TBuilder,TConvention,TDelegate}" />
    /// </summary>
    /// <seealso cref="IConventionBuilder{ICommandLineBuilder, ICommandLineConvention, CommandLineConventionDelegate}" />
    public interface ICliBuilder : IConventionBuilder<ICliBuilder, ICliConvention, CliConventionDelegate>
    {
        /// <summary>
        /// Builds the specified entry assembly.
        /// </summary>
        /// <param name="entryAssembly">The entry assembly.</param>
        /// <returns>ICommandLine.</returns>
        Parser Build(Assembly? entryAssembly = null);
    }
}