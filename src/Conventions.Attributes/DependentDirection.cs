namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// Defines the dependency direction of the given type
    /// </summary>
    public enum DependencyDirection
    {
        /// <summary>
        /// Order the convention to be run before this one.
        /// </summary>
        DependsOn,

        /// <summary>
        /// Order the convention to be run after this one.
        /// </summary>
        DependentOf
    }
}