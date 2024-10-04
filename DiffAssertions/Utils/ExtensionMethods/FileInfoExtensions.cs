using System;
using System.IO;
using System.Text;

namespace TestHelpers.DiffAssertions
{
    /// <summary>
    /// Extension mehtods that makes it easer to work with FileInfo instances.
    /// </summary>
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Reads all text from a file.
        /// </summary>
        /// <param name="fileInfo">Info about the file you want to read all text from</param>
        public static string ReadAllText(this FileInfo fileInfo)
        {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));
            if (!fileInfo.Exists) throw new FileNotFoundException($"The file {fileInfo.FullName} does not exist (so it isn't possible to read all text from it)!");
            
            return File.ReadAllText(fileInfo.FullName, Encoding.UTF8);
        }

        /// <summary>
        /// Writes text to a file.
        /// </summary>
        /// <param name="fileInfo">Info about the file you want to write to</param>
        /// <param name="contents">The text you want to write</param>
        public static void WriteAllText(this FileInfo fileInfo, string contents)
        {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));
            
            File.WriteAllText(fileInfo.FullName, contents, Encoding.UTF8);
            fileInfo.Refresh();
        }
    }
}