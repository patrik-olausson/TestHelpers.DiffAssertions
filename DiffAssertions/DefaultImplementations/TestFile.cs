using System;
using System.IO;
using System.Text;

namespace TestHelpers.DiffAssertions.DefaultImplementations
{
    internal class TestFile : ITestFile
    {
        private readonly FileInfo _fileInfo;
        private readonly Encoding _encoding;

        public string FullName => _fileInfo.FullName;
        public string Name => _fileInfo.Name;
        public string Contents => _fileInfo.ReadAllText(_encoding);
        public Encoding Encoding => _encoding ?? _fileInfo.GetEncoding();

        public TestFile(FileInfo fileInfo, Encoding encoding = null)
        {
            _fileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
            _encoding = encoding;
        }
    }
}