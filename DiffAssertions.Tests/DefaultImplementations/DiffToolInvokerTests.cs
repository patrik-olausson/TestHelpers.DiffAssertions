using System;
using TestHelpers.DiffAssertions.DefaultImplementations;
using Xunit;

namespace DefaultImplementations.DiffToolInvokerTests
{
    public class IsOnBuildServer
    {
        [Fact(Skip = "Only works when running the test on a local machine (that does not have any of the build server markers)")]
        public void GivenThatNoneOfTheBuildServerIndicatorsAreThere_ThenItReturnsFalse()
        {
            Assert.False(DiffToolInvoker.IsOnBuildServer());
        }

        [Fact(Skip = "Changes an environment variable....")]
        public void GivenThatTheDisableDiffAssertionsEnvronmentVariableExist_ThenItReturnsTrue()
        {
            try
            {
                Environment.SetEnvironmentVariable("DISABLE_DIFF_ASSERTIONS", "0");
                Assert.True(DiffToolInvoker.IsOnBuildServer());
            }
            finally
            {
                Environment.SetEnvironmentVariable("DISABLE_DIFF_ASSERTIONS", null);
            }
            
        }
    }
}