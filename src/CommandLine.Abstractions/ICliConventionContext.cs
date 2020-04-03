using System;
using System.CommandLine;
using System.CommandLine.Builder;
using JetBrains.Annotations;
using Rocket.Surgery.Conventions;
using Rocket.Surgery.Conventions.Reflection;

namespace Rocket.Surgery.Extensions.CommandLine
{
    /// <summary>
    /// ICommandLineConventionContext
    /// Implements the <see cref="IConventionContext" />
    /// </summary>
    /// <seealso cref="IConventionContext" />
    public interface ICliConventionContext : IConventionContext
    {
        /// <summary>
        /// Gets the assembly provider.
        /// </summary>
        /// <value>The assembly provider.</value>
        [NotNull] IAssemblyProvider AssemblyProvider { get; }

        /// <summary>
        /// Gets the assembly candidate finder.
        /// </summary>
        /// <value>The assembly candidate finder.</value>
        [NotNull] IAssemblyCandidateFinder AssemblyCandidateFinder { get; }
        
        /// <summary>
        /// The root application command
        /// </summary>
        [NotNull] CommandLineBuilder Builder { get; }
    }
}