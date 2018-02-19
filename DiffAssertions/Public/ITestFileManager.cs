namespace TestHelpers.DiffAssertions
{
    public interface ITestFileManager
    {
        /// <summary>
        /// Gets the file with the specified name.
        /// If the file is missing (the first time the test is run)
        /// a new empty file is created.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        ITestFile GetExpectedFile(string fileName);

        /// <summary>
        /// Creates a new temporary expected file in the current testrun directory.
        /// </summary>
        /// <param name="expectedValue">The contents of the file</param>
        /// <param name="fileName">An optional name of the file. If no name is provided
        /// a Guid will be used.</param>
        /// <returns></returns>
        ITestFile CreateTemporaryExpectedFile(string expectedValue, string fileName = null);

        /// <summary>
        /// Creates a new actual file alongside the expected file.
        /// </summary>
        /// <param name="expectedFileFullName">The full name of the expected file</param>
        /// <param name="actualValue">The contents of the file</param>
        /// <returns></returns>
        ITestFile CreateActualFile(ITestFile expectedFileFullName, string actualValue);
    }
}