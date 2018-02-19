using System;
using Microsoft.Extensions.Configuration;
using TestHelpers.DiffAssertions.Settings;

namespace DiffAssertions.Settings
{
    internal class ConfigurationBuilderBasedSettings : IDiffAsserterSettings
    {
        private readonly IConfigurationRoot _config = new ConfigurationBuilder()
            .AddJsonFile("diff-assertions.json")
            .Build();

        public string RootFolder => _config["RootFolder"];
        public TestFrameworkIdentifier TestFramework => GetConfiguredTestFramework();
        public string DiffTool => _config["DiffTool"];
        public string DiffToolArgsFormat => _config["DiffToolArgsFormat"];

        private TestFrameworkIdentifier GetConfiguredTestFramework()
        {
            try
            {
                var configurationValue = _config["TestFramework"];
                if (string.IsNullOrWhiteSpace(configurationValue))
                    throw new Exception("Configuration value missing!");

                Enum.TryParse(
                    configurationValue,
                    true,
                    out TestFrameworkIdentifier testFrameworkIdentifier);

                return testFrameworkIdentifier;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get configuration value for TestFramework", ex);
            }
        }
    }
}