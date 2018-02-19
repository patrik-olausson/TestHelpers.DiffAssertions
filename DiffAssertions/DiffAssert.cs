using DiffAssertions.Settings;
using TestHelpers.DiffAssertions.DefaultImplementations;

namespace TestHelpers.DiffAssertions
{
    /// <summary>
    /// Assertion class that makes it possible to compare two string values
    /// and show the difference in a diff comparison tool.
    /// </summary>
    public static class DiffAssert
    {
        private static readonly DiffAsserter DiffAsserter;

        static DiffAssert()
        {
            var settings = new ConfigurationBuilderBasedSettings();
            DiffAsserter = new DiffAsserter(
                new MultiTestFrameworkAsserter(settings.TestFramework),
                new DiffToolInvoker(settings.DiffTool, settings.DiffToolArgsFormat),
                new TestFileManager(settings.RootFolder));
        }

        /// <summary>
        /// Compares two strings and shows the diff in the specified tool.
        /// </summary>
        /// <param name="expected">The value you expect</param>
        /// <param name="actual">The actual value</param>
        /// <param name="nameOfFilesIfDiff">The name you want to give the files created to be able to use the diff tool.
        /// If you don't specify a name a Guid will be used as a name and it will be harder to understand what test
        /// it was that failed. The diff files are created in a separate folder (DiffFiles) in the testrun directory.</param>
        public static void Equals(string expected, string actual, string nameOfFilesIfDiff = null)
        {
            DiffAsserter.CompareStrings(expected, actual, nameOfFilesIfDiff);
        }

        /// <summary>
        /// Compares the content of a file (with the expected content) with a string that is the actual value.
        /// </summary>
        /// <param name="nameOfFileWithExpectedResult">The file name with all folders relative to the test project root but without the .expected.txt suffix. 
        /// Example: "SomeDirectory/TheFileName"</param>
        /// <param name="actualValue">The actual value produced by the test</param>
        public static void ThatExpectedFileContentsEqualsActualValue(string nameOfFileWithExpectedResult, string actualValue)
        {   
            DiffAsserter.CompareExpectedFileWithActualValue(nameOfFileWithExpectedResult, actualValue);
        }

        /// <summary>
        /// Compares the content of a file (with the expected content) with a string that is the actual value
        /// by using a fluent API. 
        /// </summary>
        /// <param name="nameOfFileWithExpectedResult">The file name with all folders relative to the test project root but without the .expected.txt suffix. 
        /// Example: "SomeDirectory/TheFileName"</param>
        /// <returns>A file content comparer that has the Equals method where you provide the actual value.</returns>
        public static FileContentComparer ThatContentsOf(string nameOfFileWithExpectedResult)
        {
            return new FileContentComparer(DiffAsserter, nameOfFileWithExpectedResult);
        }

        public static IDiffAsserter CreateInstance(
            ITestFrameworkAsserter testFrameworkAsserter,
            IDiffTool diffTool,
            ITestFileManager fileManager)
        {
            return new DiffAsserter(
                testFrameworkAsserter,
                diffTool,
                fileManager);
        }
    }
}