# Introduction

This is a library that makes it easy to use comparisons of large strings in your tests.
It helps you to easily find out what is different by opening a diff comparison tool like
WinMerge. Normal equility assertions in tests are not that great of handling large texts
and letting you get an overview of all the differences.
A very useful way to assert object state is to serialize the object to JSON and use DiffAssert to
store a file with the expected result.

## New in version 2.0
If you don't want to use text and files to compare complex objects there is a new way added to the library that
lets you assert the state of an object directly. By using an extension method on top of FluentAssertions it is
possible to make assertions like this:

```csharp
var original = CreateReferenceObject();
var updated = CreateReferenceObject();
updated.Name = "UpdatedName";
updated.Address.City = "UpdatedCity";

updated.Should().HaveValuesAsExpected(
      new
      {
          Name = "UpdatedName"
      },
      original,
      excludePropertyNames: new []
      {
          "Address.City"
      });
```
You can read more about it at the end of the getting started guide in the section [API Primer - Verifying object state](#api-primer---verifying-object-state)

# API Primer - DiffAssert using files

# Getting Started

1. Install the nuget package TestHelpers.DiffAssertions in your unit test project.
2. Update (or add the file if it wasn't added automatically along with the NuGet package) the settings in the diff-assertions.json file
  Supported (tested) DiffTools are: WinMerge and Visual Studio. It is possible to specify multiple diff tools in the DiffTool array. The first one that is found will be used.

```json
{
  "DiffTool": ["C:/Program Files/WinMerge/WinMergeU.exe"],
  "DiffToolArgsFormat": "{0} {1}"
}
```

3. Make sure the diff-assertions.json file is marked as **"Copy always"** or **"Copy if newer"** in order to make the file available when running the tests.
4. Add assertion in your test method. You can use `DiffAssert.Equals` to simply compare two strings or `DiffAssert.ThatContentsOf("MyTestSubDir/MyTestFile").Equals("Actual value")`.
5. When you create new expected files make sure to mark them as **"Copy always"** or **"Copy if newer"** in order to be able to run the tests on a build server.
6. Add \*.expected.txt.bak to your .gitignore file

There are only two operations you can perform with this library:

- `DiffAssert.Equals(string, string);`<br>
  Compares two strings using the current (specified) test frameworks Assert.Equal/Assert.AreEqual method and displays a diff tool if there are differences.
- `DiffAssert.ThatContentsOf(file name).Equals(string);`<br>
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

If you are using a dotnet framework (not core) project you have to manually include the _.expected.txt files in your project.
Remember to mark all _.expected.txt as **"Copy always"** or **"Copy if newer"** to be able to run the tests on a build server.

It is also worth noting that the testrunner always displays the result of performing a regular equals assertion (as a complement to the diff tool). When running the test on a build server it is only this assertion that will be used.

You can of course manually create the expected.txt file yourself if you already know exactly what you want the output to be and have the time and will to type it :)

To avoid cluttering the test directory with unnecessary files, all files containing the actual value are always created in special "temp" directory (named DiffAssertions) in the output directory where the test run is executed. When using `DiffAssert.Equals("A string", "Some other string");` both the expected and the actual files are created in the "temp" directory.
\*This is not true when using nUnit! See "Advanced" scenarios for more details...

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
Replaces date time values that are formated with dashes (....-..-...\*)<br>
`.ReplaceMatch(value or pattern to replace)`<br>
And finally a general Regex replace method that lets you specify whatever pattern you like

# API Primer - Verifying object state

Object comparison is an extension that is leveraging the power of FluentAssertions. It adds a layer of abstraction 
to make it easier(?) to compare objects directly. It is a possible to verify that an object has the expected state by 
comparing it against another object, a "template object". If you only want to verify a subset of properties you 
can use an anonymous object.

When verifying that an object has been updated as expected it is also possible to verify that only the subset of 
expected properties was updated by providing the original object as input. It is also possible to explicitly 
exclude selected properties from the assertion. 

```csharp
var original = CreateReferenceObject();
var updated = CreateReferenceObject();
updated.Name = "UpdatedName";
updated.Address.City = "UpdatedCity";

updated.Should().HaveValuesAsExpected(
      new
      {
          Name = "UpdatedName"
      },
      original,
      excludePropertyNames: new []
      {
          "Address.City"
      });
```

It is also possible to exclude properties by using lambda expressions or provide a "template object" (anonymous object).
    
```csharp
// Exclude properties by using lambda expressions
updated.Should().HaveValuesAsExpected(
      new
      {
          Name = "UpdatedName"
      },
      original,
      x => x.Address.City);

// Exclude properties by providing a "template object"
updated.Should().HaveValuesAsExpected(
      new
      {
          Name = "UpdatedName"
      },
      original,
      new
      {
          Address = new
          {
              City = string.Empty //Could be whatever value, it's not used
          }
      });
```

Check out the unit tests in ObjectDiffAssertionExtensionsTests.cs for more examples.