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

# API Primer
There are only two operations you can perform with this library:
* `DiffAssert.Equals(string, string);`<br>
Compares two strings using the current (specified) test frameworks Assert.Equal/Assert.AreEqual method and displays a diff tool if there are differences.
* `DiffAssert.ThatContentsOf(file name).Equals(string);`<br>
Basically does the same thing as Equals but the expected value is stored in a file. If you don't like the fluent syntax it is possible to use the alternative method `DiffAssert.ThatExpectedFileContentsEqualsActualValue(file name, string);`

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

var result = sut.DoWorkThatProducesResultObject("Some arguments");

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
If you are using a standard dotnet project you have to include the *.expected.txt files in your project.
Remember to mark all *.expected.txt as **"Copy always"** or **"Copy if newer"** to be able to run the tests on a build server.

It is also worth noting that the testrunner always displays the result of performing a regular equals assertion (as a complement to the diff tool). When running the test on a build server it is only this assertion that will be used.

You can of course manually create the expected.txt file yourself if you already know exactly what you want the output to be and have the time and will to type it :)

To avoid cluttering the test directory with unnecessary files, all files containing the actual value are always created in special "temp" directory (named DiffAssertions) in the output directory where the test run is executed. When using `DiffAssert.Equals("A string", "Some other string");` both the expected and the actual files are created in the "temp" directory.

# Some useful tools
As the example shows it is very useful to serialize objects to JSON in order to assert the state. To make this as easy as possible there are two simple extension methods you can use:<br>
`.ToJsonString(object)`<br>
Serializes an object to a JSON string
`.FromJsonString<T>(string with json)`<br>
Deserializes an object from a JSON string

When serializing objects to JSON (or simply working with dynamic strings) it is often a challenge to control Guid and DateTime values that are created during the test. To avoid failing tests due to changes in these kind of values there are some helpful string extension mehtods you can use:<br>
`.ReplaceGuids()`<br>
Replaces Guids that are formatted with dashes (........-....-....-....-............)<br>
`.RepalceJsonFormatedDateTime()`<br>
Replaces date time values that are formated with dashes (....-..-...*)<br>
`.ReplaceMatch(value or pattern to replace)`<br>
And finally a general Regex replace method that lets you specify whatever pattern you like

# "Advanced" scenarios
It is possible to create an instance of the DiffAsserter and replace some of the key parts with your own implementations. This could be useful if you find yourself in a situation where the test framework you are using is not supported. To still be able to use DiffAssertions you implement the interface ITestFrameworkAsserter.
```csharp
public class MyTestFrameworkAsserter : ITestFrameworkAsserter
{
	public void Equals(string expected, string actual)
	{
		Assert.Equal(expected, actual);
	}
}
```
And then you create an instance by calling:<br>
```csharp
var diffAssert = DiffAssert.CreateInstance(new MyTestFrameworkAsserter());

diffAssert.CompareExpectedFileWithActualValue("fileName", "actualValue");
```