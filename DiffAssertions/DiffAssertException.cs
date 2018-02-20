using System;
using System.Text;

namespace TestHelpers.DiffAssertions
{
    /// <summary>
    /// Exception that is thrown when a diff is detected
    /// </summary>
    public class DiffAssertException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expectedFile"></param>
        /// <param name="actualFile"></param>
        /// <param name="innerException"></param>
        public DiffAssertException(
            ITestFile expectedFile, 
            ITestFile actualFile, 
            Exception innerException) 
            : base(new StringBuilder()
                .AppendLine("Diff detected!")
                .AppendLine(expectedFile.FullName)
                .AppendLine(actualFile.FullName)
                .ToString(), 
                innerException)
        {
        }
    }
}