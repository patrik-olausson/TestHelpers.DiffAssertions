namespace TestHelpers.DiffAssertions
{
    /// <summary>
    /// The core functionality of this assembly. It provides the 
    /// possibility to assert two strings or the contents of a file against
    /// an actual value.
    /// </summary>
    public interface IDiffAsserter
    {
        /// <summary>
        /// Compares the content of a file (the expected result) with a string that is the actual value.
        /// </summary>
        /// <param name="nameOfFileWithExpectedResult"></param>
        /// <param name="actualValue"></param>
        void CompareExpectedFileWithActualValue(string nameOfFileWithExpectedResult, string actualValue);

        /// <summary>
        /// Compares two strings and shows the diff in the specified tool.
        /// </summary>
        /// <param name="expectedValue"></param>
        /// <param name="actualValue"></param>
        /// <param name="fileName"></param>
        void CompareStrings(string expectedValue, string actualValue, string fileName = null);
    }
}