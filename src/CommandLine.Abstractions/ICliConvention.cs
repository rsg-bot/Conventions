using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Extensions.CommandLine
{
    /// <summary>
    /// ILoggingConvention
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{ICommandLineConventionContext}" />
    public interface ICliConvention : IConvention<ICliConventionContext> { }
}