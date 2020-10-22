using System;
using TestHelpers.DiffAssertions;
using Xunit;

namespace DiffAssertTests
{
    public class Equals
    {
        [Fact]
        public void GivenTwoEqualValues_ItPassesTheTest()
        {
            DiffAssert.Equals("Identical", "Identical");
        }

        [Fact]
        public void GivenThatTheActualValueEqualsTheValueInTheExpectedFile_ItPassesTheTest()
        {
            DiffAssert
                .ThatContentsOf("DiffAssertTests/FileWithContentThatShouldMatchTheActualValue")
                .Equals("The actual value");
        }

        [Fact(Skip = "Not able to run in build because it always fails...")]
        //[Fact]
        public void GivenTwoDifferentValues_ItShowsTheDiffTool()
        {
            DiffAssert.Equals("Expected", "Actual");
        }

        [Fact(Skip = "Not able to run in build because it always fails...")]
        //[Fact]
        public void GivenThatTheExpectedValueIsDifferentThanTheExpectedFile_ItShowsTheDiffTool()
        {
            DiffAssert.ThatContentsOf("DiffAssertTests/LocalDiffTest").Equals("Not the expected value");
        }

        [Fact(Skip = "Changes an environment variable....")]
        public void GivenThatTheTestIsRunOnTheBuildServer_ThenItChecksForTheExpectedFilesInTheOutputDirectory()
        {
            try
            {
                Environment.SetEnvironmentVariable("DISABLE_DIFF_ASSERTIONS", "0");

                DiffAssert
                    .ThatContentsOf("DiffAssertTests/BuildServerTest")
                    .Equals("Works on build server if the file is marked as Copy always");
            }
            finally
            {
                Environment.SetEnvironmentVariable("DISABLE_DIFF_ASSERTIONS", null);
            }
        }
    }
}