using System;
using System.Text;

namespace TestHelpers.DiffAssertions
{
    public class DiffAssertException : Exception
    {
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