namespace TestHelpers.DiffAssertions
{
    /// <summary>
    /// Abstraction of the current test framework.
    /// </summary>
    public interface ITestFrameworkAsserter
    {
        /// <summary>
        /// Invokes the current test frameworks Equal, AreEqual, Equals method
        /// and if that fails it is time to display the diff tool...
        /// </summary>
        void Equals(string expected, string actual);
    }
}