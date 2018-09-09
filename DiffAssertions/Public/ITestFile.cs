namespace TestHelpers.DiffAssertions
{
    /// <summary>
    /// The most important information about a test file
    /// </summary>
    public interface ITestFile
    {
        /// <summary>
        /// The complete path of the file
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// The name of the file without the path
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The contents of the file
        /// </summary>
        string Contents { get; }
    }
}