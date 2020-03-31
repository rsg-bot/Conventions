namespace Rocket.Surgery.Conventions
{
    /// <summary>
    /// The underlying host type (currently only checked for test host)
    /// This is so that conventions can avoid adding behaviors that might cause issues with unit testing such as writing to the
    /// console or debug pipes, or ensuring that something is configured in a correct way for testing.
    /// Careful consideration is needed to make sure that your system doesn't behave extremely differently in a live scenario
    /// vs a testing scenario
    /// </summary>
    public enum HostType
    {
        /// <summary>
        /// No hast type has been defined
        /// </summary>
        Undefined,

        /// <summary>
        /// This is a live application
        /// This also may apply to in memory integration tests running through the HostBuilder infrastructure
        /// </summary>
        Live,

        /// <summary>
        /// This is a unit test context
        /// This is so that conventions can avoid adding behaviors that might cause issues with unit testing such as writing to the
        /// console or debug pipes, or ensuring that something is configured in a correct way for testing.
        /// Careful consideration is needed to make sure that your system doesn't behave extremely differently in a live scenario
        /// vs a testing scenario
        /// </summary>
        UnitTestHost
    }
}