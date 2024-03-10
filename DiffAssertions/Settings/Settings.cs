using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using TestHelpers.DiffAssertions;

namespace DiffAssertions.Settings
{
    internal class ConfigurationBuilderBasedSettings : IDiffAsserterSettings
    {
        private readonly IConfigurationRoot _config = new ConfigurationBuilder()
            .AddJsonFile("diff-assertions.json")
            .Build();

        public string RootFolder { get; }
        public string DiffTool { get; }
        public string DiffToolArgsFormat => _config["DiffToolArgsFormat"];

        public ConfigurationBuilderBasedSettings()
        {
            var rootFolder = 
                TryToGetValueFromConfigFile() ??
                TryToFindRootFolderIfTestRunIsInBinFolder() ??
                Directory.GetCurrentDirectory();

            if(string.IsNullOrWhiteSpace(rootFolder))
                throw new Exception("Unable to load root folder candidates from settings file. You must specify at least one path or the solution name.");

            RootFolder = rootFolder;
            DiffTool = SelectDiffToolPathToUse();
        }

        private string SelectDiffToolPathToUse()
        {
            var diffToolPathCandidates = _config.GetSection("DiffTool").GetChildren().Select(x => x.Value);
            var firstCandidateThatExist = diffToolPathCandidates.FirstOrDefault(File.Exists);

            return firstCandidateThatExist ??
                   "None of the specified paths to the DiffTool exe file is valid.";
        }

        private string TryToGetValueFromConfigFile()
        {
            try
            {
                var rootFolderCandidates = _config.GetSection("RootFolders").GetChildren().Select(x => x.Value).ToArray();
                if (rootFolderCandidates.Length == 0)
                    return null;

                foreach (var rootFolderCandidate in rootFolderCandidates)
                {
                    if (Directory.Exists(rootFolderCandidate))
                    {
                        return rootFolderCandidate;
                    }
                }

                throw new DirectoryNotFoundException("None of the specified root folders exist, please check the paths specified in the settings file.");
            }
            catch (DirectoryNotFoundException)
            {
                throw;
            }
            catch
            {
                return null;
            }
        }

        internal static string TryToFindRootFolderIfTestRunIsInBinFolder()
        {
            try
            {
                return Directory.GetCurrentDirectory().GetPathBeforeFolder("bin");
            }
            catch
            {
                return null;
            }
        }
    }
}