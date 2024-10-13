using DiffAssertions.Tests;
using FluentAssertions;
using TestHelpers.DiffAssertions;
using Xunit;
using Xunit.Abstractions;

namespace DiffAsserterTests.ObjectDiffAssertionExtensionsTests;

public class GetMemberName : ObjectDiffAssertionTestHarness
{
    public GetMemberName(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void GivenMemberNameOfSimpleProperty_ThenItReturnsTheExpectedName()
    {
        var result = ObjectDiffAssertionExtensions.GetMemberName<Person>(x => x.Age);
        
        result.Should().Be("Age");
    }
    
    [Fact]
    public void GivenMemberNameOfSimpleLeafProp_ThenItReturnsTheExpectedName()
    {
        var result = ObjectDiffAssertionExtensions.GetMemberName<Person>(x => x.Address.City);
        
        result.Should().Be("Address.City");
    }
    
    [Fact]
    public void GivenMemberNameOfLeafPropInList_ThenItReturnsTheExpectedName()
    {
        var result = ObjectDiffAssertionExtensions.GetMemberName<Person>(x => x.ContactInfos[0].Value);
        
        result.Should().Be("ContactInfos[0].Value");
    }
}

public class DefineExclusions : ObjectDiffAssertionTestHarness
{
    public DefineExclusions(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void GivenDefinitonsOfSomePropertiesToExclude_ThenItReturnsTheExpectedNamesEvenForLeafProps()
    {
        var referenceObject = CreateReferenceObject();
        var result = referenceObject.DefineExclusions(
            offer => offer.Name,
            offer => offer.Address.StreetInfo.Address2,
            offer => offer.ContactInfos[1].Value);

        result.Should().BeEquivalentTo(
            "Name",
            "Address.StreetInfo.Address2",
            "ContactInfos[1].Value");
    }
}

public class HaveValuesAsExpected : ObjectDiffAssertionTestHarness
{
    public HaveValuesAsExpected(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void GivenUpdateOfTwoPropertiesThatMatchesTemplate_ThenItAssertsSuccess()
    {
        var person = CreateReferenceObject();
        person.Name = "UpdatedName";
        person.Address.City = "UpdatedCity";

        person.Should().HaveValuesAsExpected(
            new
            {
                Name = "UpdatedName",
                Address = new { City = "UpdatedCity" }
            });
    }
    
    [Fact]
    public void GivenUpdateOfTwoPropertiesThatDoesNotMatchesTemplate_ThenItReportsError()
    {
        var person = CreateReferenceObject();
        person.Name = "UpdatedName";
        person.Address.City = "UpdatedCity";

        AssertThatErrorWasReported(
            () => person.Should().HaveValuesAsExpected(
            new
            {
                Name = "NewName",
                Address = new { City = "NewCity" }
            }),
            "Expected property root.Name to be \"NewName\" with a length of 7, but \"UpdatedName\" has a length of 11, differs near \"Upd\" (index 0)");
    }
    
    [Fact]
    public void GivenUpdateOfTwoPropertiesButOnlyOneIsMatchedUsingTemplate_ThenItAssertsSuccess()
    {
        var person = CreateReferenceObject();
        person.Name = "UpdatedName";
        person.Address.City = "UpdatedCity";

        person.Should().HaveValuesAsExpected(
            new
            {
                Name = "UpdatedName",
            });
    }
    
    [Fact]
    public void GivenUpdateOfTwoPropertiesThatMatchesTemplateAndNoOtherChangesIsConfirmedByUsingOriginalObject_ThenItAssertsSuccess()
    {
        var original = CreateReferenceObject();
        var updated = CreateReferenceObject();
        updated.Name = "UpdatedName";
        updated.Address.City = "UpdatedCity";

        updated.Should().HaveValuesAsExpected(
            new
            {
                Name = "UpdatedName",
                Address = new { City = "UpdatedCity" }
            },
            original);
    }
    
    [Fact]
    public void GivenUpdateOfMorePropertiesThanMatchedByTemplateAndOriginalObjectIsUsedToConfirm_ThenItReportsError()
    {
        var original = CreateReferenceObject();
        var updated = CreateReferenceObject();
        updated.Name = "UpdatedName";
        updated.Address.City = "UpdatedCity";

        AssertThatErrorWasReported(
            () => updated.Should().HaveValuesAsExpected(
                new
                {
                    Name = "UpdatedName"
                },
                original),
            "Expected property root.Address.City to be \"Anytown\" with a length of 7 because the expected updates should be the only changes made to the original object, but \"UpdatedCity\" has a length of 11, differs near \"Upd\" (index 0)");
    }
    
    [Fact]
    public void GivenUpdateOfMorePropertiesThanMatchedByTemplateAndOriginalObjectIsUsedToConfirmButUnmatchedPropertyIsExcludedByUsingExplicitName_ThenItAssertsSuccess()
    {
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
    }
    
    [Fact]
    public void GivenUpdateOfMorePropertiesThanMatchedByTemplateAndOriginalObjectIsUsedToConfirmButUnmatchedPropertyIsExcludedByUsingPropertyExpression_ThenItAssertsSuccess()
    {
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
            x => x.Address.City);
    }
    
    [Fact]
    public void GivenUpdateOfMorePropertiesThanMatchedByTemplateAndOriginalObjectIsUsedToConfirmButUnmatchedPropertyIsExcludedByUsingObjectTemplate_ThenItAssertsSuccess()
    {
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
            new
            {
                Address = new
                {
                    City = string.Empty //Could be whatever value, it's not used
                }
            });
    }
    
    [Fact]
    public void GivenUpdateOfObjectInListThatMatchesTemplate_ThenItAssertsSuccess()
    {
        var updated = CreateReferenceObject();
        updated.ContactInfos[1].Value = "UpdatedEmail";

        updated.Should().HaveValuesAsExpected(
            new
            {
                ContactInfos = new[]
                {
                    new { Value = "123-456-7890" },
                    new { Value = "UpdatedEmail" }
                }
            });
    }
}