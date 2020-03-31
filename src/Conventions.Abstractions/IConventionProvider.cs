using System;
using System.Collections.Generic;
using Rocket.Surgery.Conventions.Scanners;

namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// IConventionProvider
    /// </summary>
    public interface IConventionProvider
    {
        /// <summary>
        /// Gets this instance.  filtered by host type
        /// </summary>
        /// <typeparam name="TContribution">The type of the contribution.</typeparam>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="hostType">The host type.</param>
#pragma warning disable CA1716
        IEnumerable<DelegateOrConvention> Get<TContribution, TDelegate>(HostType hostType = HostType.Undefined)
            where TContribution : IConvention
            where TDelegate : Delegate;
#pragma warning restore CA1716

        /// <summary>
        /// Gets a all the conventions from the provider filtered by host type
        /// </summary>
        /// <param name="hostType">The host type.</param>
        IEnumerable<DelegateOrConvention> GetAll(HostType hostType = HostType.Undefined);
    }
}