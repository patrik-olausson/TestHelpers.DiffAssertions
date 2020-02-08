using System.IO;
using System.Reflection;
using NUnit.Framework;
using TestHelpers.DiffAssertions;
using TestHelpers.DiffAssertions.DefaultImplementations;

namespace DiffAssertions._461.nUnitTests
{
    [SetUpFixture]
    public class NUnitDiffAssertionSetup
    {
        [OneTimeSetUp]
        public void GlobalSetupOfWorkingDirectoryToEnableDiffAssertions()
        {
            //NUnit does not set the working directory, which is a problem when using DiffAssertions that relies on it...
            string currentDirectory;
            if (DiffToolInvoker.IsOnBuildServer())
            {
                currentDirectory = GetTestRunDirectory();
            }
            else
            {
                currentDirectory = GetSourceCodeDirectory();
            }

            Directory.SetCurrentDirectory(currentDirectory);
        }

        private string GetTestRunDirectory()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var testRunDirectory = assembly.Location.Replace($"{assembly.GetName().Name}.dll", "");

            return testRunDirectory;
        }

        private string GetSourceCodeDirectory()
        {
            return Assembly.GetExecutingAssembly().Location.GetPathBeforeFolder("bin");
        }
    }
}