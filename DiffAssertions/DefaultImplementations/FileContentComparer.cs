using System;

namespace TestHelpers.DiffAssertions.DefaultImplementations
{
    /// <summary>
    /// Class that makes it possible to provida a simple fluent api when doing diff asserts
    /// using a file
    /// </summary>
    public class FileContentComparer
    {
        private readonly IDiffAsserter _diffAsserter;
        private readonly string _nameOfFileWithExpectedResult;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diffAsserter"></param>
        /// <param name="nameOfFileWithExpectedResult"></param>
        public FileContentComparer(IDiffAsserter diffAsserter, string nameOfFileWithExpectedResult)
        {
            if (string.IsNullOrWhiteSpace(nameOfFileWithExpectedResult))
            {
                throw new ArgumentException("You must specify the name of the file that contains the expected value. Include folders relative to the root but exclude the .expected.txt suffix.", nameof(nameOfFileWithExpectedResult));
            }

            _diffAsserter = diffAsserter ?? throw new ArgumentNullException(nameof(diffAsserter));
            _nameOfFileWithExpectedResult = nameOfFileWithExpectedResult;
        }

        /// <summary>
        /// Asserts if the value equals the contents of the specified expected file
        /// </summary>
        /// <param name="actualValue">The value you want to compare to the expected file</param>
        public void Equals(string actualValue)
        {
            _diffAsserter.CompareExpectedFileWithActualValue(_nameOfFileWithExpectedResult, actualValue);
        }
    }
}