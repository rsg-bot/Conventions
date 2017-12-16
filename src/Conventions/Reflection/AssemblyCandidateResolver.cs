#if !NETSTANDARD1_3
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Rocket.Surgery.Conventions.Reflection
{
    class AssemblyCandidateResolver
    {
        private readonly ILogger _logger;
        private readonly IDictionary<string, Dependency> _dependencies;

        public AssemblyCandidateResolver(IReadOnlyList<Assembly> assemblies, ISet<string> referenceAssemblies, ILogger logger)
        {
            _logger = logger;
            var dependenciesWithNoDuplicates = new Dictionary<string, Dependency>(StringComparer.OrdinalIgnoreCase);
            foreach (var assembly in assemblies)
            {
                RecursiveAddDependencies(assembly, referenceAssemblies, dependenciesWithNoDuplicates);
            }
            _dependencies = dependenciesWithNoDuplicates;
        }

        private void RecursiveAddDependencies(
            Assembly assembly,
            ISet<string> referenceAssemblies,
            IDictionary<string, Dependency> dependenciesWithNoDuplicates)
        {
            var key = assembly.GetName().Name;
            if (!dependenciesWithNoDuplicates.ContainsKey(key))
            {
                dependenciesWithNoDuplicates.Add(key, CreateDependency(assembly, referenceAssemblies));
            }

            foreach (var dependency in assembly.GetReferencedAssemblies())
            {
                if (dependency.Name.StartsWith("System.") || dependency.Name.StartsWith("mscorlib") || dependency.Name.StartsWith("Microsoft."))
                    continue;

                Assembly dependentAssembly;
                try
                {
                    dependentAssembly = Assembly.Load(dependency);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(0, e, "Unable to load assembly {Name}", dependency.Name);
                    continue;
                }
                RecursiveAddDependencies(dependentAssembly, referenceAssemblies, dependenciesWithNoDuplicates);
            }
        }

        private Dependency CreateDependency(Assembly library, ISet<string> referenceAssemblies)
        {
            var classification = DependencyClassification.Unknown;
            if (referenceAssemblies.Contains(library.GetName().Name))
            {
                classification = DependencyClassification.Reference;
            }

            return new Dependency(library, classification);
        }

        private DependencyClassification ComputeClassification(string dependency)
        {
            // Prevents issues with looking at system assemblies
            if (dependency.StartsWith("System.") || dependency.StartsWith("mscorlib") || dependency.StartsWith("Microsoft."))
            {
                return DependencyClassification.NotCandidate;
            }

            if (!_dependencies.TryGetValue(dependency, out var candidateEntry) || candidateEntry.Assembly == null)
            {
                return DependencyClassification.Unknown;
            }

            if (candidateEntry.Classification != DependencyClassification.Unknown)
            {
                return candidateEntry.Classification;
            }

            var classification = DependencyClassification.NotCandidate;

            foreach (var candidateDependency in candidateEntry.Assembly.GetReferencedAssemblies())
            {
                var dependencyClassification = ComputeClassification(candidateDependency.Name);
                if (dependencyClassification == DependencyClassification.Candidate ||
                    dependencyClassification == DependencyClassification.Reference)
                {
                    classification = DependencyClassification.Candidate;
                    break;
                }
            }

            candidateEntry.Classification = classification;

            return classification;
        }

        public IEnumerable<Dependency> GetCandidates()
        {
            foreach (var dependency in _dependencies)
            {
                if (ComputeClassification(dependency.Key) == DependencyClassification.Candidate && dependency.Value.Assembly != null)
                {
                    yield return dependency.Value;
                }
            }
        }

        internal class Dependency
        {
            public Dependency(Assembly assemblyName, DependencyClassification classification)
            {
                Assembly = assemblyName;
                Classification = classification;
            }

            public Assembly Assembly { get; }

            public DependencyClassification Classification { get; set; }

            public override string ToString()
            {
                return $"AssemblyName: {Assembly.GetName().Name}, Classification: {Classification}";
            }
        }
    }
}
#endif