using TestHelpers.DiffAssertions.Settings;

namespace DiffAssertions.Settings
{
    internal interface IDiffAsserterSettings
    {
        string RootFolder { get; }
        TestFrameworkIdentifier TestFramework { get; }
        string DiffTool { get; }
        string DiffToolArgsFormat { get; }
    }
}