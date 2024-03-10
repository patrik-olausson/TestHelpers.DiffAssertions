namespace DiffAssertions.Settings
{
    internal interface IDiffAsserterSettings
    {
        string RootFolder { get; }
        string DiffTool { get; }
        string DiffToolArgsFormat { get; }
    }
}