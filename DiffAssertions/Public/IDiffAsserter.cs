namespace TestHelpers.DiffAssertions
{
    public interface IDiffAsserter
    {
        void CompareExpectedFileWithActualValue(string nameOfFileWithExpectedResult, string actualValue);
        void CompareStrings(string expectedValue, string actualValue, string fileName = null);
    }
}