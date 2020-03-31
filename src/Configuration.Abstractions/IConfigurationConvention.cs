using Rocket.Surgery.Conventions;

namespace Rocket.Surgery.Extensions.Configuration
{
    /// <summary>
    /// ILoggingConvention
    /// Implements the <see cref="IConvention{TContext}" />
    /// </summary>
    /// <seealso cref="IConvention{IConfigurationConventionContext}" />
    public interface IConfigConvention : IConvention<IConfigConventionContext> { }
}