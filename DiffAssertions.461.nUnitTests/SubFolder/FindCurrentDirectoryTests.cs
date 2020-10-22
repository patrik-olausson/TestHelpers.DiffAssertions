using NUnit.Framework;
using TestHelpers.DiffAssertions;

namespace DiffAssertions._461.nUnitTests.SubFolder
{
    [TestFixture]
    public class FindCurrentDirectoryTests
    {
        [Test]
        public void WhenRunningTestItIsPossibleToDetermineTheCorrectRootPaths()
        {
            //This works because the NUnitDiffAssertionsSetup class sets the working directory...
            DiffAssert
                .ThatContentsOf("SubFolder/IsItPossibleToFindOutTheRootFolderEvenWithNUnit")
                .Equals("Yeah! It is possible!");
        }
    }
}
