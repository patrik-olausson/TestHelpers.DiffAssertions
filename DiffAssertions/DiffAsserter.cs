using System;

namespace TestHelpers.DiffAssertions
{
    internal class DiffAsserter : IDiffAsserter
    {
        private readonly ITestFrameworkAsserter _testFrameworkAsserter;
        private readonly IDiffTool _diffTool;
        private readonly ITestFileManager _testFileManager;

        public DiffAsserter(
            ITestFrameworkAsserter testFrameworkAsserter,
            IDiffTool diffTool,
            ITestFileManager fileManager)
        {
            _testFrameworkAsserter = testFrameworkAsserter ?? throw new ArgumentNullException(nameof(testFrameworkAsserter));
            _diffTool = diffTool ?? throw new ArgumentNullException(nameof(diffTool));
            _testFileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        }

        public void CompareExpectedFileWithActualValue(string nameOfFileWithExpectedResult, string actualValue)
        {
            var expectedFile = _testFileManager.GetExpectedFile(nameOfFileWithExpectedResult);
            
            try
            {
                _testFrameworkAsserter.Equals(expectedFile.Contents, actualValue);
            }
            catch (Exception ex)
            {
                if (_diffTool.IsUnableToUse)
                    throw;

                var actualFile = _testFileManager.CreateActualFile(expectedFile, actualValue);
                _diffTool.CompareFiles(expectedFile, actualFile);

                throw new DiffAssertException(expectedFile, actualFile, ex);
            }
        }

        public void CompareStrings(string expectedValue, string actualValue, string fileName = null)
        {
            try
            {
                _testFrameworkAsserter.Equals(expectedValue, actualValue);
            }
            catch (Exception ex)
            {
                if (_diffTool.IsUnableToUse)
                    throw;

                var expectedFile = _testFileManager.CreateTemporaryExpectedFile(expectedValue, fileName);
                var actualFile = _testFileManager.CreateActualFile(expectedFile, actualValue);
                _diffTool.CompareFiles(expectedFile, actualFile);

                throw new DiffAssertException(expectedFile, actualFile, ex);
            }
        }
    }
}