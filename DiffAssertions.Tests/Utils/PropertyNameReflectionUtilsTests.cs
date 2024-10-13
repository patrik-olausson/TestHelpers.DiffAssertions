using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DiffAssertions.Tests;
using FluentAssertions;
using TestHelpers.DiffAssertions.Utils;
using Xunit;
using Xunit.Abstractions;

namespace DiffAsserterTests.Utils.FilHelperTests.PropertyNameReflectionUtilsTests
{
    public class GetLeafMemberName : PropertyNameReflectionUtilsTestHarness
    {
        public GetLeafMemberName(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void GivenMemberDirectlyOnClass_ThenItReturnsTheName()
        {
            var memberExpression = GetMemberExpressionBody<Person>(r => r.Age);

            var result = PropertyNameReflectionUtils.GetLeafMemberName(memberExpression);

            result.Should().Be("Age");
        }
        
        [Fact]
        public void GivenLeafMemberOneLevelDeep_ThenItReturnsTheQualifiedPropertyName()
        {
            var memberExpression = GetMemberExpressionBody<Person>(r => r.Address.ZipCode);

            var result = PropertyNameReflectionUtils.GetLeafMemberName(memberExpression);

            result.Should().Be("Address.ZipCode");
        }
        
        [Fact]
        public void GivenLeafMemberTwoLevelsDeep_ThenItReturnsTheQualifiedPropertyName()
        {
            var memberExpression = GetMemberExpressionBody<Person>(r => r.Address.StreetInfo.Address1);

            var result = PropertyNameReflectionUtils.GetLeafMemberName(memberExpression);

            result.Should().Be("Address.StreetInfo.Address1");
        }
        
        [Fact]
        public void GivenLeafMemberThatIsInCollectionUsingLinqFirst_ThenItReturnsThePropertyNameUsingIndexedSyntaxUsefulForFluentAssertions()
        {
            var memberExpression = GetMemberExpressionBody<Person>(r => r.ContactInfos.First().Type);

            var result = PropertyNameReflectionUtils.GetLeafMemberName(memberExpression);

            result.Should().Be("ContactInfos[0].Type");
        }
        
        [Fact]
        public void GivenLeafMemberThatIsInCollectionUsingLinqElementAt_ThenItReturnsThePropertyNameUsingIndexedSyntaxUsefulForFluentAssertions()
        {
            var memberExpression = GetMemberExpressionBody<Person>(r => r.ContactInfos.ElementAt(1).Type);

            var result = PropertyNameReflectionUtils.GetLeafMemberName(memberExpression);

            result.Should().Be("ContactInfos[1].Type");
        }
        
        [Fact]
        public void GivenLeafMemberThatIsInCollectionUsingIndexer_ThenItReturnsThePropertyNameUsingIndexedSyntaxUsefulForFluentAssertions()
        {
            var memberExpression = GetMemberExpressionBody<Person>(r => r.ContactInfos[1].Type);

            var result = PropertyNameReflectionUtils.GetLeafMemberName(memberExpression);

            result.Should().Be("ContactInfos[1].Type");
        }
    }

    public class GetPropertyNames : PropertyNameReflectionUtilsTestHarness
    {
        public GetPropertyNames(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void GivenPropertyInfoForTypeWithNoNestedProperties_ThenReturnsTheExpectedPropertyNames()
        {
            var streetInfo = typeof(Address).GetProperties().First(x => x.Name == nameof(Address.StreetInfo));

            var result = PropertyNameReflectionUtils.GetPropertyNames(streetInfo);

            result.Should().BeEquivalentTo("StreetInfo.Address1", "StreetInfo.Address2");
        }
        
        [Fact]
        public void GivenPropertyInfoForTypeWithNestedProperties_ThenReturnsTheExpectedPropertyNames()
        {
            var typeWithPerson = new { Person = CreateReferenceObject() };
            var personInfo = typeWithPerson.GetType().GetProperties().First(x => x.Name == "Person");
            
            var result = PropertyNameReflectionUtils.GetPropertyNames(personInfo);

            result.Should().BeEquivalentTo(
                "Person.Name",
                "Person.Age",
                "Person.Address.StreetInfo.Address1",
                "Person.Address.StreetInfo.Address2",
                "Person.Address.City",
                "Person.Address.ZipCode",
                "Person.ContactInfos.Type",
                "Person.ContactInfos.Value");
        }
    }

    public class ContainsPropertyPath
    {
        [Fact]
        public void GivenCollectionThatHasMatchingValue_ThenItReturnsTrue()
        {
            var values = new List<string> { "Person.Name", "Person.Age" };

            var result = PropertyNameReflectionUtils.ContainsPropertyPath(values, "Person.Age");
            
            result.Should().BeTrue();
        }
        
        [Fact]
        public void GivenCollectionThatHasNoMatchingValue_ThenItReturnsFalse()
        {
            var values = new List<string> { "Person.Name", "Person.Age" };

            var result = PropertyNameReflectionUtils.ContainsPropertyPath(values, "SomeUnknownProperty");
            
            result.Should().BeFalse();
        }
        
        [Fact]
        public void GivenCollectionThatIsNull_ThenItReturnsFalse()
        {
            var result = PropertyNameReflectionUtils.ContainsPropertyPath(null, "SomeProperty");
            
            result.Should().BeFalse();
        }
        
        [Fact]
        public void GivenCollectionThatIsEmpty_ThenItReturnsFalse()
        {
            var result = PropertyNameReflectionUtils.ContainsPropertyPath(new List<string>(), "SomeProperty");
            
            result.Should().BeFalse();
        }
    }

    public class PropertyNameReflectionUtilsTestHarness : ObjectDiffAssertionTestHarness
    {
        public PropertyNameReflectionUtilsTestHarness(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected Expression GetMemberExpressionBody<T>(Expression<Func<T, object>> expression)
        {
            return expression.Body;
        }
    }
}

