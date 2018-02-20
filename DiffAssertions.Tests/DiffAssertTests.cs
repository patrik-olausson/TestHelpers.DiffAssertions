using TestHelpers.DiffAssertions;
using Xunit;

namespace DiffAssertTests
{
    public class Equals
    {
        [Fact(Skip = "Not able to run in build because it always fails...")]
        public void GivenTwoDifferentValues_ItShowsTheDiffTool()
        {
            DiffAssert.Equals("Expected", "Actual");
        }
    }
}