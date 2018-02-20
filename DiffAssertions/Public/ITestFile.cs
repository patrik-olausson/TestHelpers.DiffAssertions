using System.Text;

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
        /// The contents of the file
        /// </summary>
        string Contents { get; }

        /// <summary>
        /// The encoding to use when working with the file
        /// </summary>
        Encoding Encoding { get; }
    }
}