namespace TestHelpers.DiffAssertions
{
    public interface ITestFrameworkAsserter
    {
        void Equals(string expected, string actual);
    }
}