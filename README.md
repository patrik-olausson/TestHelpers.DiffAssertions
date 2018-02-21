# Introduction 
This is a library that makes it easy to use comparisons of large strings in your tests.
It helps you to easily find out what is different by opening a diff comparison tool like
WinMerge. Normal equility assertions in tests are not that great of handling large texts
and letting you get an overview of all the differences.
A very useful way to assert object state is to serialize the object to JSON and use DiffAssert to
store a file with the expected result.

# Getting Started
1. Install the nuget package TestHelpers.DiffAssertions in your unit test project.
2. Update the settings in the diff-assertions.json file.
	- The root folder of your test project is used to create and update files with the expected result.
3. Make sure the diff-assertions.json file is marked as **"Copy always"** or **"Copy if newer"** in order to make the file available when running the tests.
4. Add assertion in your test method. You can use  `DiffAssert.Equals` to simply compare two strings or `DiffAssert.ThatContentsOf("MyTestSubDir/MyTestFile").Equals("Actual value")`.
5. When you create new expected files make sure to include them in your project and marking them as **"Copy always"** or **"Copy if newer"** in order to be able to run the tests on a build server.

# How it works
DiffAssertions always starts by performing a regular equals assertion by using the test framework you are using (specified by you in the diff-assertions.json file). If the assertion fails it creates one or two files (depending on if the expected file already exists or not) with the strings and invokes a diff tool to display the differences.

If the test is run on a build server the diff tool should not be displayed and this is prevented by checking for some common environment variables to try to identify if it is a build server or not. If the default checks doesn't seem to be working for you it is possible to simply add a new environment variable named `DISABLE_DIFF_ASSERTIONS` to tell diff assertions to avoid starting the diff tool.

## Example of the workflow when working with expected value files
Lets say you have a test where you call some service to do some work and you want to verify that the result is what you expect.

For this example the test is in a file that is placed in a sub folder to the test project:
```
TestProject
|-SubFolder
	|-ServiceTests.cs
```
```csharp
var sut = CreateSystemUnderTest();

var result = sut.DoWorkThatProducesAnResultObject("Some arguments");

DiffAssert
	.ThatContentsOf("SubFolder/DescriptiveNameOfTheExpectedResultForThisTest")
	.Equals(result.ToJsonString());
```
When this test is run for the first time the file with the expected result will not exist. DiffAssertions will create the file (with an empty content) and will trigger the test to fail. When the test fails the specified diff tool will be opened and display the expected value (to the left) and the actual value (to the right).

After inspecting the actual value and deciding if it looks correct or not you can easily copy the actual value to the expected value (and even make some manual changes if it wasn't exactly what you wanted) and save the expected file.

When the test has been completed a new file has been added:
```
TestProject
|-SubFolder
	|-ServiceTests.cs
	|-DescriptiveNameOfTheExpectedResultForThisTest.expected.txt
```
If you are using a standard dotnet project you have to display hidden files and include the *.expected.txt files in your project.
Remember to mark all *.expected.txt as **"Copy always"** or **"Copy if newer"** to be able to run the tests on a build server.

It is also worth noting that the testrunner should display the result of performing a regular equals assertion (as a complement to the diff tool). When running the test on a build server it is only this assertion that will be used.

You can of course manually create the expected.txt file yourself if you already know exactly what you want the output to be and have the time and will to type it :)

To avoid cluttering the test directory with unnecessary files all files containing the actual value are always created in special "temp" directory (named DiffAssertions) in the output directory where the test run is executed. When using DiffAssert.Equals("A string", "Some other string") both the expected and the actual files are created in the "temp" directory.