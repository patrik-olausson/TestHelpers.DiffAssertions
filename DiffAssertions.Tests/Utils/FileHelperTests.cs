using System;
using FluentAssertions;
using TestHelpers.DiffAssertions.Utils;
using Xunit;

namespace DiffAsserterTests.Utils.FilHelperTests
{
    public class ExtractDirectoryNameFromPath : FileHelperTestHarness
    {
        [Theory]
        [InlineData(@"\SomeFolder\SomeSubFolder\SomeFile.txt", "/SomeFolder/SomeSubFolder")]
        [InlineData("/SomeFolder/SomeSubFolder/SomeFile.txt", "/SomeFolder/SomeSubFolder")]
        [InlineData("SomeFolder/SomeSubFolder/SomeFile.txt", "SomeFolder/SomeSubFolder")]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData(" ", "")]
        void GivenDifferentPaths_ThenItReturnsTheExpectedCleanedPath(
            string path,
            string expectedCleanedPath)
        {
            var result = FileHelper.ExtractDirectoryNameFromPath(path);
            
            AssertPath(expectedCleanedPath, result);
        }
    }

    public class GetRelativePathToCurrentTestDirectory : FileHelperTestHarness
    {
        [Theory]
        [InlineData(
            @"C:\RootFolder\SubFolder\SolutionFolder\TestProjectFolder\SubFolder1\SubFolder2\SubFolder3\NameOfFileThatCallsMethod.cs", 
            "TestProjectFolder",
            "SubFolder1/SubFolder2/SubFolder3")]
        [InlineData(
            @"C:\RootFolder\TestProjectFolder\SubFolder1\NameOfFileThatCallsMethod.cs", 
            "TestProjectFolder",
            "SubFolder1")]
        [InlineData(
            @"C:\TestProjectFolder\NameOfFileThatCallsMethod.cs", 
            "TestProjectFolder",
            "")]
        [InlineData(
            "C:/RootFolder/SubFolder/SolutionFolder/TestProjectFolder/SubFolder1/SubFolder2/SubFolder3/NameOfFileThatCallsMethod.cs", 
            "TestProjectFolder",
            "SubFolder1/SubFolder2/SubFolder3")]
        [InlineData(
            "C:/RootFolder/TestProjectFolder/SubFolder1/NameOfFileThatCallsMethod.cs", 
            "TestProjectFolder",
            "SubFolder1")]
        [InlineData(
            "C:/TestProjectFolder/NameOfFileThatCallsMethod.cs", 
            "TestProjectFolder",
            "")]
        public void GivenDifferentFilePathsAndTestDirectoryNames_ThenItExtractsTheExpectedRelativePath(
            string callerFilePath,
            string testProjectName,
            string expectedRelativePath)
        {
            var result = FileHelper.GetRelativePathToCurrentTestDirectory(callerFilePath, testProjectName);
            
            AssertPath(expectedRelativePath, result);
        }

        [Fact]
        public void GivenCallerFilePathThatDoesNotContainTheSpecifiedTestProjectName_ThenItThrowsAnException()
        {
            var callerFilePath = "C:/RootFolder/TestProjectFolder/SubFolder1/NameOfFileThatCallsMethod.cs";
            var testProjectName = "NameThatIsNotPartOfTheFilePath";
            
            Assert.Throws<ArgumentException>(() => FileHelper.GetRelativePathToCurrentTestDirectory(callerFilePath, testProjectName));
        }
    }
    
    public class GetRelativePathToFileInTestProject : FileHelperTestHarness
    {
        [Theory]
        [InlineData(
            @"C:\RootFolder\SubFolder\SolutionFolder\TestProjectFolder\SubFolder1\SubFolder2\SubFolder3\NameOfFileThatCallsMethod.cs", 
            "TestProjectFolder",
            "CustomFilename.txt",
            "SubFolder1/SubFolder2/SubFolder3/CustomFilename.txt")]
        [InlineData(
            @"C:\RootFolder\TestProjectFolder\SubFolder1\NameOfFileThatCallsMethod.cs", 
            "TestProjectFolder",
            "CustomFilename.txt",
            "SubFolder1/CustomFilename.txt")]
        [InlineData(
            @"C:\TestProjectFolder\NameOfFileThatCallsMethod.cs", 
            "TestProjectFolder",
            "CustomFilename.txt",
            "CustomFilename.txt")]
        [InlineData(
            "C:/RootFolder/SubFolder/SolutionFolder/TestProjectFolder/SubFolder1/SubFolder2/SubFolder3/NameOfFileThatCallsMethod.cs", 
            "TestProjectFolder",
            "CustomFilename.txt",
            "SubFolder1/SubFolder2/SubFolder3/CustomFilename.txt")]
        [InlineData(
            "C:/RootFolder/TestProjectFolder/SubFolder1/NameOfFileThatCallsMethod.cs", 
            "TestProjectFolder",
            "CustomFilename.txt",
            "SubFolder1/CustomFilename.txt")]
        [InlineData(
            "C:/TestProjectFolder/NameOfFileThatCallsMethod.cs", 
            "TestProjectFolder",
            "CustomFilename.txt",
            "CustomFilename.txt")]
        public void GivenDifferentFilePathsAndTestDirectoryNames_ThenItExtractsTheRelativePathAndCombinesItWithTheFilenameAsExpected(
            string callerFilePath,
            string testProjectName,
            string filename,
            string expectedRelativePath)
        {
            var result = FileHelper.GetRelativePathToFileInTestProject(
                callerFilePath,
                testProjectName,
                filename);
            
            AssertPath(expectedRelativePath, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GivenThatTheFilenameIsNotSpecified_ThenItThrowsAnArgumentException(string filename)
        {
            Assert.Throws<ArgumentException>(() => FileHelper.GetRelativePathToFileInTestProject("C:/TestProjectFolder/NameOfFileThatCallsMethod.cs", "TestProjectFolder", filename));
        }
    }

    public class GetAbsolutePathToFile : FileHelperTestHarness
    {
        [Theory]
        [InlineData(
            "C:/TestProjectFolder/NameOfFileThatCallsMethod.cs", 
            "CustomFilename.txt",
            "C:/TestProjectFolder/CustomFilename.txt")]
        [InlineData(
            "C:/TestProjectFolder/NameOfFileThatCallsMethod.cs", 
            "SubFolder/CustomFilename.txt",
            "C:/TestProjectFolder/SubFolder/CustomFilename.txt")]
        public void GivenAbsolutePathToAFileAndASpecifiedFilename_ThenItReplacesTheFilenameAsExpected(
            string pathToReferenceFile,
            string filenameThatCouldContainSubdirectories,
            string expectedAbsolutePath)
        {
            var result = FileHelper.GetAbsolutePathToFile(pathToReferenceFile, filenameThatCouldContainSubdirectories);
            
            AssertPath(expectedAbsolutePath, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GivenThatTheCallerFilePathIsInvalid_ThenItThrowsAnArgumentException(string callerFilePath)
        {
            var ex = Assert.Throws<ArgumentException>(() => FileHelper.GetAbsolutePathToFile(callerFilePath, "CustomFilename.txt"));
            ex.Message.Should().StartWith("Not possible to extract");
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void GivenThatTheFilenameIsInvalid_ThenItThrowsAnArgumentException(string filename)
        {
            var ex = Assert.Throws<ArgumentException>(() => FileHelper.GetAbsolutePathToFile("C:/TestProjectFolder/NameOfFileThatCallsMethod.cs", filename));
            ex.Message.Should().StartWith("Value cannot be null or whitespace.");
        }
    }

    public class GetRelativePath : FileHelperTestHarness
    {
        [Theory]
        [InlineData(
            @"C:\RootFolder1\SubFolder\SolutionFolder\TestProjectFolder\SubFolder1\SubFolder2\SubFolder3\NameOfFileThatCallsMethod.cs",
            "TestProjectFolder",
            "SubFolder1/SubFolder2/SubFolder3/NameOfFileThatCallsMethod.cs")]
        [InlineData(
            @"C:\RootFolder2\SubFolder\SolutionFolder\TestProjectFolder\SubFolder1\SubFolder2\SubFolder3", 
            "TestProjectFolder",
            "SubFolder1/SubFolder2/SubFolder3")]
        public void GivenAbsolutePathToAFileOrDirectoryAndASpecifiedRootDirectory_ThenItReplacesTheFilenameAsExpected(
            string absolutePathToFileOrDirectory,
            string rootDirectoryName,
            string expectedRelativePath)
        {
            var result = FileHelper.GetRelativePath(absolutePathToFileOrDirectory, rootDirectoryName);
            
            AssertPath(expectedRelativePath, result);
        }
    }

    public class FileHelperTestHarness
    {
        protected void AssertPath(string expectedPath, string actualPath)
        {
            //Make sure test is able to run on different OS
            actualPath = GetPathThatWorksOnDifferentOperatingSystems(actualPath);
            
            Assert.Equal(expectedPath, actualPath);
        }
        
        private string GetPathThatWorksOnDifferentOperatingSystems(string path)
        {
            const char WindowsDirectorySeparator = '\\';
            const char UnixDirectorySeparator = '/';

            return path.Replace(WindowsDirectorySeparator, UnixDirectorySeparator);
        }
    }
};