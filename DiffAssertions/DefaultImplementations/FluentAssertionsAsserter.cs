using FluentAssertions;

namespace TestHelpers.DiffAssertions.DefaultImplementations
{
    internal class FluentAssertionsAsserter : ITestFrameworkAsserter
    {
        public void Equals(string expected, string actual)
        {
            actual.Should().Be(expected);
        }
    }
}