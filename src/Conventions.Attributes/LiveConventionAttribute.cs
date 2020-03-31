using System;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Defines this convention as one that only runs during live usage to avoid unit tests
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class LiveConventionAttribute : Attribute, IHostBasedConvention
    {
        HostType IHostBasedConvention.HostType => HostType.Live;
    }
}