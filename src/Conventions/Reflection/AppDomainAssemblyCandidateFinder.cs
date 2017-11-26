﻿#if !NETSTANDARD1_3
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Assembly candidate finder that uses <see cref="AppDomain"/>
    /// </summary>
    public class AppDomainAssemblyCandidateFinder : IAssemblyCandidateFinder
    {
        private readonly AppDomain _appDomain;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyContextAssemblyCandidateFinder" /> class.
        /// </summary>
        /// <param name="appDomain">The application domain.</param>
        /// <param name="logger">The logger.</param>
        public AppDomainAssemblyCandidateFinder(AppDomain appDomain = null, ILogger logger = null)
        {
            _appDomain = appDomain ?? AppDomain.CurrentDomain;
            _logger = logger ?? NullLogger.Instance;
        }

        /// <inheritdoc />
        public IEnumerable<Assembly> GetCandidateAssemblies(IEnumerable<string> candidates)
        {
            return GetCandidateLibraries(candidates.ToArray())
                .Where(x => x != null);
        }

        internal IEnumerable<Assembly> GetCandidateLibraries(string[] candidates)
        {
            if (!candidates.Any())
            {
                return Enumerable.Empty<Assembly>();
            }

            var candidatesResolver = new AssemblyCandidateResolver(_appDomain.GetAssemblies(), new HashSet<string>(candidates, StringComparer.OrdinalIgnoreCase));
            return candidatesResolver.GetCandidates();
        }
    }
}
#endif
