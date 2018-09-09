using System;
using System.IO;

namespace TestHelpers.DiffAssertions.DefaultImplementations
{
    internal class TestFile : ITestFile
    {
        private readonly FileInfo _fileInfo;

        public string FullName => _fileInfo.FullName;
        public string Name => _fileInfo.Name;
        public string Contents => _fileInfo.ReadAllText();

        public TestFile(FileInfo fileInfo)
        {
            _fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
        }
    }
}