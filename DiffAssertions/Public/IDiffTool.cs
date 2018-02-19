namespace TestHelpers.DiffAssertions
{
    public interface IDiffTool
    {
        void CompareFiles(ITestFile expected, ITestFile actual);
        bool IsUnableToUse { get; }
    }
}