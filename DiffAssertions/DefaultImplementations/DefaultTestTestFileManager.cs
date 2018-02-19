using System;
using System.IO;

namespace TestHelpers.DiffAssertions.DefaultImplementations
{
    public class TestFileManager : ITestFileManager
    {
        private readonly string _rootFolder;
        private readonly DirectoryInfo _tempDirectoryForStringComparisons = new DirectoryInfo("DiffAssertions");

        public TestFileManager(string rootFolder)
        {
            if (string.IsNullOrWhiteSpace(rootFolder))
            {
                throw new System.ArgumentException("You must specify the path to test project root folder", nameof(rootFolder));
            }

            _rootFolder = rootFolder;
        }

        public ITestFile GetExpectedFile(string fileName)
        {
            var fullName = Path.Combine(_rootFolder, $"{fileName}.expected.txt");
            var expectedFile = new FileInfo(fullName); 

            if (!expectedFile.Exists)
            {
                expectedFile.WriteAllText("");
            }

            return new TestFile(expectedFile);
        }

        public ITestFile CreateTemporaryExpectedFile(string expectedValue, string nameOfFileWithExpectedResult = null)
        {
            if (nameOfFileWithExpectedResult == null)
                nameOfFileWithExpectedResult = Guid.NewGuid().ToString();

            var fullName = Path.Combine(_tempDirectoryForStringComparisons.FullName, $"{nameOfFileWithExpectedResult}.expected.txt");
            var expectedFile = new FileInfo(fullName);
            expectedFile.WriteAllText(expectedValue);

            return new TestFile(expectedFile);
        }

        public ITestFile CreateActualFile(ITestFile expectedFile, string actualValue)
        {
            var fileName = expectedFile.FullName.Replace(".expected.txt", ".actual.txt");
            var actualFile = new FileInfo(fileName);
            actualFile.WriteAllText(actualValue);

            return new TestFile(actualFile);
        }
    }
}