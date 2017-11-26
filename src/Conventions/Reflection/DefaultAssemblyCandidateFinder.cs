﻿#if !NETSTANDARD1_3
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Rocket.Surgery.Conventions.Reflection
{
    /// <summary>
    /// Default assembly candidate finder that uses a list of assemblies
    /// </summary>
    public class DefaultAssemblyCandidateFinder : IAssemblyCandidateFinder
    {
        private readonly List<Assembly> _assemblies;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyContextAssemblyCandidateFinder" /> class.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <param name="logger">The logger.</param>
        public DefaultAssemblyCandidateFinder(IEnumerable<Assembly> assemblies, ILogger logger = null)
        {
            _assemblies = assemblies.ToList();
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

            var candidatesResolver = new AssemblyCandidateResolver(_assemblies, new HashSet<string>(candidates, StringComparer.OrdinalIgnoreCase));
            return candidatesResolver.GetCandidates();
        }
    }
}
#endif
