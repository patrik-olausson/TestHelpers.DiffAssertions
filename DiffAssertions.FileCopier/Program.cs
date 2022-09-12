namespace DiffAssertions.FileCopier;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var projectDirectory = new DirectoryInfo(args[0]);
            var destinationDirectory = new DirectoryInfo(args[1]);

            CopyAllExpectedFilesFromTestProjectDirectory(projectDirectory, destinationDirectory);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static void CopyAllExpectedFilesFromTestProjectDirectory(DirectoryInfo projectDirectory,
        DirectoryInfo destinationDirectory)
    {
        var expectedFiles = projectDirectory.GetFiles("*.expected.txt", SearchOption.AllDirectories);
        foreach (var expectedFile in expectedFiles)
        {
            if (!InBinFolder(expectedFile.FullName))
            {
                var destinationFilePath =
                    $"{destinationDirectory.FullName}{expectedFile.FullName.Replace(projectDirectory.FullName, "")}"; 
                var destinationFile = new FileInfo(destinationFilePath);
                if (!destinationFile.Exists && !string.IsNullOrWhiteSpace(destinationFile.DirectoryName))
                {
                    Directory.CreateDirectory(destinationFile.DirectoryName);
                }
                else
                {
                    File.SetAttributes(destinationFile.FullName, FileAttributes.Normal);
                }

                File.Copy(
                    expectedFile.FullName,
                    destinationFile.FullName,
                    true);
                Console.WriteLine($"Copied file {destinationFile}");
            }
        }
    }

    private static bool InBinFolder(string expectedFileFullName)
    {
        return expectedFileFullName.Contains(@"\bin\", StringComparison.OrdinalIgnoreCase);
    }
}