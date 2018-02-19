using System;

namespace TestHelpers.DiffAssertions.DefaultImplementations
{
    public class FileContentComparer
    {
        private readonly IDiffAsserter _diffAsserter;
        private readonly string _nameOfFileWithExpectedResult;

        public FileContentComparer(IDiffAsserter diffAsserter, string nameOfFileWithExpectedResult)
        {
            if (string.IsNullOrWhiteSpace(nameOfFileWithExpectedResult))
            {
                throw new ArgumentException("You must specify the name of the file that contains the expected value. Include folders relative to the root but exclude the .expected.txt suffix.", nameof(nameOfFileWithExpectedResult));
            }

            _diffAsserter = diffAsserter ?? throw new ArgumentNullException(nameof(diffAsserter));
            _nameOfFileWithExpectedResult = nameOfFileWithExpectedResult;
        }

        public void Equals(string actualValue)
        {
            _diffAsserter.CompareExpectedFileWithActualValue(_nameOfFileWithExpectedResult, actualValue);
        }
    }
}