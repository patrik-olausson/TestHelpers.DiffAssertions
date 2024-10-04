using System.IO;
using System.Runtime.CompilerServices;
using TestHelpers.DiffAssertions.Utils;

namespace TestHelpers.DiffAssertions
{
    /// <summary>
    /// Base class that should simplify the use of DiffAssertions in test harnesses.
    /// </summary>
    public class DiffAssertTestHarness
    {
        /// <summary>
        /// Helper method that makes it easier to use DiffAssertions.
        /// </summary>
        /// <param name="testProjectDirectoryName">The name of the test project directory (normally the same as the test project).
        /// The name is used to be able to infer the root directory of files on disc relative to the test project.</param>
        /// <param name="fileNameForExpectedValue">The name of the file, without any path and extensions.
        /// Path to the directory and the .expected.txt suffix will be added automatically.
        /// </param>
        /// <param name="actualValue">The value produced by the current version of the code.</param>
        /// <param name="callerFilePath">The absolute path to a file that is in the root directory you want to use
        /// (normally the directory of the test that is currently executing).
        /// The best way is to rely on using the CallerFilePath attribute to capture a call from the test that is
        /// currently executing and reference files in the same directory or a subdirectory structure.</param>
        protected virtual void DiffAssert(
            string testProjectDirectoryName,
            string fileNameForExpectedValue,
            string actualValue,
            [CallerFilePath] string callerFilePath = "")
        {
            var pathToRootFolder = GetRelativePathToFolder(callerFilePath, testProjectDirectoryName);
            
            DiffAssertions.DiffAssert
                .ThatContentsOf(Path.Combine(pathToRootFolder, fileNameForExpectedValue))
                .Equals(actualValue);
        }

        /// <summary>
        /// Gets the relative path to a folder using the test project directory as root folder.
        /// </summary>
        /// <param name="callerFilePath">The absolute path to a file that is in the root directory you want to use
        /// (normally the directory of the test that is currently executing).
        /// The best way is to rely on using the CallerFilePath attribute to capture a call from the test that is
        /// currently executing and reference files in the same directory or a subdirectory structure.</param>
        /// <param name="testProjectDirectoryName">The name of the test project directory (normally the same as the test project).
        /// The name is used to be able to infer the root directory of files on disc relative to the test project.</param>
        protected string GetRelativePathToFolder(string callerFilePath, string testProjectDirectoryName)
        {
            return FileHelper.GetRelativePathToCurrentTestDirectory(callerFilePath, testProjectDirectoryName);
        }

        /// <summary>
        /// Gets the relative path to a file using the test project director as root folder.
        /// </summary>
        /// <param name="callerFilePath">The absolute path to a file that is in the root directory you want to use
        /// (normally the directory of the test that is currently executing).
        /// The best way is to rely on using the CallerFilePath attribute to capture a call from the test that is
        /// currently executing and reference files in the same directory or a subdirectory structure.</param>
        /// <param name="testProjectDirectoryName">The name of the test project directory (normally the same as the test project).
        /// The name is used to be able to infer the root directory of files on disc relative to the test project.</param>
        /// <param name="filename">The name of the file you want to append to the path.
        /// This could (and probably should) include a path to a subdirectory structure.
        /// NOTE: when using subdirectories in the path, use / to ensure it will work for both Windows and Linux.</param>
        /// <returns></returns>
        protected string GetRelativePathToFile(string callerFilePath, string testProjectDirectoryName, string filename)
        {
            return FileHelper.GetRelativePathToFileInTestProject(
                callerFilePath,
                testProjectDirectoryName,
                filename);
        }

        /// <summary>
        /// Gets information about a specified file.
        /// </summary>
        /// <param name="testProjectDirectoryName">The name of the test project directory (normally the same as the test project).
        /// The name is used to be able to infer the root directory of files on disc relative to the test project.</param>
        /// <param name="filename">The name of the file you want to append to the path.
        /// This could (and probably should) include a path to a subdirectory structure.
        /// NOTE: when using subdirectories in the path, use / to ensure it will work for both Windows and Linux.</param>
        /// <param name="directory">Path from the relative root directory.</param>
        /// <param name="fileExtension">File extension (.json, .txt, etc)</param>
        /// <param name="callerFilePath">The absolute path to a file that is in the root directory you want to use
        /// (normally the directory of the test that is currently executing).
        /// The best way is to rely on using the CallerFilePath attribute to capture a call from the test that is
        /// currently executing and reference files in the same directory or a subdirectory structure.</param>
        /// <returns></returns>
        public FileInfo GetFileInfo(
            string testProjectDirectoryName,
            string filename,
            string directory = "[TestFiles]",
            string fileExtension = ".json",
            [CallerFilePath] string callerFilePath = "")
        {
            var filePath = GetRelativePathToFile(
                callerFilePath,
                testProjectDirectoryName,
                Path.Combine(directory, $"{filename}{fileExtension}"));

            return new FileInfo(filePath);
        }

        /// <summary>
        /// Reads all text from a file and adds error message that should be helpful when the file is not found.
        /// </summary>
        /// <param name="testProjectDirectoryName"></param>
        /// <param name="filename">The name of the file you want to append to the path.
        /// This could (and probably should) include a path to a subdirectory structure.
        /// NOTE: when using subdirectories in the path, use / to ensure it will work for both Windows and Linux.</param>
        /// <param name="callerFilePath">The absolute path to a file that is in the root directory you want to use
        /// (normally the directory of the test that is currently executing).
        /// The best way is to rely on using the CallerFilePath attribute to capture a call from the test that is
        /// currently executing and reference files in the same directory or a subdirectory structure.</param>
        /// <returns></returns>
        protected string GetContentFromFile(
            string testProjectDirectoryName,
            string filename,
            [CallerFilePath] string callerFilePath = "")
        {
            var filePath = GetRelativePathToFile(
                callerFilePath,
                testProjectDirectoryName,
                filename);
            
            return FileHelper.GetContentFromFile(filePath);
        }
    }
}