using System.Text;

namespace TestHelpers.DiffAssertions
{
    public interface ITestFile
    {
        string FullName { get; }
        string Contents { get; }
        Encoding Encoding { get; }
    }
}