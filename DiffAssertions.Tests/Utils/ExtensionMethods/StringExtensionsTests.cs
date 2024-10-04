using TestHelpers.DiffAssertions;
using Xunit;

namespace StringExtensionsTests
{
    public class GetPathBeforeFolder
    {
        [Fact]
        public void GivenAPathWithADepthOfFourAndThePartUntilTheSecondFolderIsRequested_ThenItReturnsThePathWithoutEndingSlash()
        {
            var path = @"C:\First\Second\Third\Fourth";

            var result = path.GetPathBeforeFolder("Second");

            Assert.Equal(@"C:\First", result);
        }

        [Fact]
        public void GivenAPathWithADepthOfFourAndThePartUntilTheThirdFolderIsRequested_ThenItReturnsThePathWithoutEndingSlash()
        {
            var path = @"C:\First\Second\Third\Fourth";

            var result = path.GetPathBeforeFolder("Third");

            Assert.Equal(@"C:\First\Second", result);
        }

        [Fact]
        public void GivenAPathWithADepthOfFourAndThePartUntilTheFirstFolderIsRequested_ThenItReturnsNull()
        {
            var path = @"First\Second\Third\Fourth";

            var result = path.GetPathBeforeFolder("First");

            Assert.Null(result);
        }

        [Fact]
        public void GivenAPathWithADepthOfFourThatAlsoHasTheDriveSpecifiedAndThePartUntilTheFirstFolderIsRequested_ThenItReturnsTheDrive()
        {
            var path = @"C:\First\Second\Third\Fourth";

            var result = path.GetPathBeforeFolder("First");

            Assert.Equal("C:", result);
        }

        [Fact]
        public void GivenAPathWithADepthOfThreeAndThePartUntilAnUnknownFolderIsRequested_ThenItReturnsNull()
        {
            var path = @"C:\First\Second\Third";

            var result = path.GetPathBeforeFolder("Not a known folder");

            Assert.Null(result);
        }
    }
}