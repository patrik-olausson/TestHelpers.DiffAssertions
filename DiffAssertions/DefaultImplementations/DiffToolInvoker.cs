using System;
using System.Diagnostics;

namespace TestHelpers.DiffAssertions.DefaultImplementations
{
    /// <summary>
    /// Class responslible for starting the diff tool
    /// </summary>
    public class DiffToolInvoker : IDiffTool
    {
        private readonly string _diffToolPath;
        private readonly string _diffToolArgsFormat;

        /// <summary>
        /// If the tests are run on a build server the diff tool should be avoided!
        /// </summary>
        /// <returns>True if there is any of the known environment variables for different build servers available.</returns>
        //TODO: Is this really going to work? Have to research and test this in more detail!
        public bool IsUnableToUse => IsOnBuildServer();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="diffToolPath">The full file name to the diff tool exe</param>
        /// <param name="diffToolArgsFormat">A format string that specifies how the file names should be passed to the diff tool</param>
        public DiffToolInvoker(string diffToolPath, string diffToolArgsFormat)
        {
            if (string.IsNullOrWhiteSpace(diffToolPath))
            {
                throw new ArgumentException("To be able to start the diff tool you must specify the full path to the exe file", nameof(diffToolPath));
            }

            if (string.IsNullOrWhiteSpace(diffToolArgsFormat))
            {
                throw new ArgumentException("To be able to start the diff tool you must specify the command line format for the two arguments", nameof(diffToolArgsFormat));
            }

            _diffToolPath = diffToolPath;
            _diffToolArgsFormat = diffToolArgsFormat;
        }

        /// <summary>
        /// Starts the configured diff tool and uses the file names as arguments
        /// </summary>
        /// <param name="expected">The expected file (will be the left argument)</param>
        /// <param name="actual">The actual file (will be the right argument)</param>
        public void CompareFiles(ITestFile expected, ITestFile actual)
        {
            var arguments = string.Format(_diffToolArgsFormat, $"\"{expected.FullName}\"", $"\"{actual.FullName}\"");
            try
            {
                Process.Start(_diffToolPath, arguments);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to launch diff tool {_diffToolPath} with arguments {arguments}. Check the paths and verify that the correct values are specified in the diff-assertions.json file.", ex);
            }
        }

        /// <summary>
        /// Checks a known set of environment variables to try to figure out the testrun is on a build server or not.
        /// </summary>
        /// <returns></returns>
        public static bool IsOnBuildServer()
        {
            return GetEnvironmentVarialbeThatIndicatesThatThisIsABuildServer() != null;
        }
        
        private static string GetEnvironmentVarialbeThatIndicatesThatThisIsABuildServer()
        {
            return Environment.GetEnvironmentVariable("DISABLE_DIFF_ASSERTIONS") ??
                   Environment.GetEnvironmentVariable("SYSTEM_TEAMPROJECT") ??
                   Environment.GetEnvironmentVariable("JENKINS_URL") ??
                   Environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME");
        }
    }
}