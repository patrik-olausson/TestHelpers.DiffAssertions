namespace TestHelpers.DiffAssertions
{
    /// <summary>
    /// Abstraction of the diff tool
    /// </summary>
    public interface IDiffTool
    {
        /// <summary>
        /// Starts the diff tool with the two files as argument
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        void CompareFiles(ITestFile expected, ITestFile actual);

        /// <summary>
        /// Decides if it is possible to use the diff tool or not.
        /// If the test is run on a build server the diff tool shouldn't 
        /// be uesed.
        /// </summary>
        bool IsUnableToUse { get; }
    }
}