using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit.Abstractions;

namespace DiffAssertions.Tests;

public class ObjectDiffAssertionTestHarness
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ObjectDiffAssertionTestHarness(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    protected void LogTestOutput(string message)
    {
        _testOutputHelper.WriteLine(message);
    }

    protected void AssertThatErrorWasReported(Action assert, string expectedErrorMessage)
    {
        try
        {
            assert();
        }
        catch (Exception e)
        {
            LogTestOutput(e.Message);
            e.Message.Should().StartWith(expectedErrorMessage);
        }
    }
    
    protected Person CreateReferenceObject()
    {
        return new Person
        {
            Name = "John Doe",
            Age = 30,
            Address = new Address
            {
                StreetInfo = new StreetInfo
                {
                    Address1 = "123 Main St",
                    Address2 = "Suite 100"
                },
                City = "Anytown",
                ZipCode = "12345"
            },
            ContactInfos = new List<ContactInfo>
            {
                new ContactInfo { Type = ContactType.Phone, Value = "123-456-7890" },
                new ContactInfo { Type = ContactType.Email, Value = "john.doe@work.com" }
            }
        };
    }
    
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Address Address { get; set; }
        public IList<ContactInfo> ContactInfos { get; set; }
    }

    public class Address
    {
        public StreetInfo StreetInfo { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }

    public class StreetInfo
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
    }

    public class ContactInfo
    {
        public ContactType Type { get; set; }
        public string Value { get; set; }
    }

    public enum ContactType
    {
        Email,
        Phone
    }
}