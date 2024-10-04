using System;
using System.IO;

namespace TestHelpers.DiffAssertions.Utils
{
    internal class FileHelper
    {
        /// <summary>
        /// Entry point for using the file helper as part of a test harness.
        /// </summary>
        /// <param name="absoluteFilePath">The absolute path to a file that is in the root directory you want to use
        /// (normally the directory of the test that is currently executing).
        /// The best way is to rely on using the CallerFilePath attribute to capture a call from the test that is
        /// currently executing and reference files in the same directory or a subdirectory structure.</param>
        /// <param name="testProjectDirectoryName">The name of the test project directory, to be used as the root
        /// directory in the relative path.</param>
        /// <returns></returns>
        public static string GetRelativePathToCurrentTestDirectory(string absoluteFilePath, string testProjectDirectoryName)
        {
            var relativePath = GetRelativePath(absoluteFilePath, testProjectDirectoryName);

            return ExtractDirectoryNameFromPath(relativePath);
        }

        /// <summary>
        /// Entry point for using the file helper as part of a test harness.
        /// </summary>
        /// <param name="absoluteFilePath">The absolute path to a file that is in the root directory you want to use
        /// (normally the directory of the test that is currently executing).
        /// The best way is to rely on using the CallerFilePath attribute to capture a call from the test that is
        /// currently executing and reference files in the same directory or a subdirectory structure.</param>
        /// <param name="testProjectDirectoryName">The name of the test project directory, to be used as the root
        /// directory in the relative path.</param>
        /// <param name="filename">The name of the file you want to append to the path.
        /// This could (and probably should) include a path to a subdirectory structure.
        /// NOTE: when using subdirectories in the path, use / to ensure it will work for both Windows and Linux.</param>
        /// <returns></returns>
        public static string GetRelativePathToFileInTestProject(
            string absoluteFilePath,
            string testProjectDirectoryName,
            string filename)
        {
            var absolutePath = GetAbsolutePathToFile(absoluteFilePath, filename);
            
            return GetRelativePath(absolutePath, testProjectDirectoryName);
        }

        /// <summary>
        /// Takes an absolute path to a reference file in the root directory of your test and replaces
        /// the filename of the reference file with the specified filename. 
        /// </summary>
        /// <param name="callerFilePath">The absolute path to a file that is in the root directory you want to use
        /// (normally the directory of the test that is currently executing).
        /// The best way is to rely on using the CallerFilePath attribute to capture a call from the test that is
        /// currently executing and reference files in the same directory or a subdirectory structure.</param>
        /// <param name="filename">The name of the file you want to append to the path.
        /// This could (and probably should) include a path to a subdirectory structure.
        /// NOTE: when using subdirectories in the path, use / to ensure it will work for both Windows and Linux.</param>
        /// <returns>An absolute path to the specified filename.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <example>
        /// <code>
        /// var callerFilePath = @"C:\Projects\TestProject\Tests\UnitTest1.cs";
        /// var filename = @"TestData\example.json";
        /// var absolutePath = FileHelper.GetAbsolutePathToFile(callerFilePath, filename);
        /// // absolutePath will be "C:\Projects\TestProject\Tests\TestData\example.json"
        /// </code>
        /// </example>
        public static string GetAbsolutePathToFile(string callerFilePath, string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filename));

            var absolutePath = ExtractDirectoryNameFromPath(callerFilePath);
            if (string.IsNullOrWhiteSpace(absolutePath))
                throw new ArgumentException($"Not possible to extract an absolute directory path from {callerFilePath}.", nameof(callerFilePath));

            return Path.Combine(absolutePath, filename);
        }

        /// <summary>
        /// Takes an absolute file path and the name of a directory in that path that you want to use as the root directory
        /// in a relative path. It then extracts the relative path to the file from the root directory by simply splitting
        /// the absolute path on the wanted root directory name.
        /// </summary>
        /// <param name="absolutePath">The absolute file path, including the filename, to a file.</param>
        /// <param name="rootDirectoryName">The name of the directory you want to use as root directory in your relative path.</param>
        /// <returns>A relative path that starts from the specified directory name.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <example>
        /// <code>
        /// var absoluteFilePath = @"C:\Projects\TestProject\Tests\UnitTest1.cs";
        /// var rootDirectoryName = @"Tests";
        /// var relativePath = FileHelper.GetRelativePathToFile(absoluteFilePath, rootDirectoryName);
        /// // relativePath will be "Tests\TestData\example.json"
        /// </code>
        /// </example>
        public static string GetRelativePath(string absolutePath, string rootDirectoryName)
        {
            if (string.IsNullOrWhiteSpace(absolutePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(absolutePath));
            if (string.IsNullOrWhiteSpace(rootDirectoryName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(rootDirectoryName));

            var parts = absolutePath.Split(new[] { rootDirectoryName }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                throw new ArgumentException("Unable to extract the relative file path based on the provided root directory name");

            return parts[1].TrimStart('\\', '/');
        }

        /// <summary>
        /// Reads all text from a file and adds error message that should be helpful when the file is not found.
        /// </summary>
        /// <param name="filePath">The complete file path to a file.</param>
        /// <returns>All text found in the specified file.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string GetContentFromFile(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
                {
                    throw new ArgumentException($"Unable to find file {Path.GetFileName(filePath)}. Configure it to be copied to the output directory (Copy If Newer or Copy Always)! Complete path: {filePath}", ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Tries to get the directory name from a path, but if the provided path is null or whitespace it returns
        /// an empty string.
        /// </summary>
        internal static string ExtractDirectoryNameFromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;

            return Path.GetDirectoryName(path);
        }
    }
}