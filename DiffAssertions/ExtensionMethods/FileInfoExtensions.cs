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
        /// <param name="encoding">The encoding you want to use when reading the file. If no
        /// encoding is specified the encoding is "guessed" by checking the BOM (or
        /// using Encoding.Default if unable to decide).</param>
        /// <returns>All contents of the file as a string</returns>
        public static string ReadAllText(this FileInfo fileInfo, Encoding encoding = null)
        {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));
            if (!fileInfo.Exists) throw new FileNotFoundException($"The file {fileInfo.FullName} does not exist (so it isn't possible to read all text from it)!");
            if (encoding == null)
            {
                encoding = fileInfo.GetEncoding();
            }

            return File.ReadAllText(fileInfo.FullName, encoding);
        }

        /// <summary>
        /// Writes text to a file.
        /// </summary>
        /// <param name="fileInfo">Info about the file you want to write to</param>
        /// <param name="contents">The text you want to write</param>
        /// <param name="encoding">The encoding you want to use. If no encoding is specified
        /// the Encoding.Default is used.</param>
        public static void WriteAllText(this FileInfo fileInfo, string contents, Encoding encoding = null)
        {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }

            File.WriteAllText(fileInfo.FullName, contents, encoding);
            fileInfo.Refresh();
        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// </summary>
        /// <param name="fileInfo">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        /// <remarks>
        /// https://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding
        /// </remarks>
        public static Encoding GetEncoding(this FileInfo fileInfo)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;

            return Encoding.Default;
        }
    }
}