using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using FluentAssertions.Primitives;
using TestHelpers.DiffAssertions.Utils;

namespace TestHelpers.DiffAssertions
{
    /// <summary>
    /// A set of extensions for FluentAssertions that makes it easier to assert that an object was updated as expected.
    /// </summary>
    public static class ObjectDiffAssertionExtensions
    {
        /// <summary>
        /// Asserts that an object was updated as expected.
        /// To ensure the that only the expected properties were updated, and all other properties have the original values,
        /// you can specify the original object for verification. 
        /// </summary>
        /// <param name="assertions">The extended FluentAssertion object (ObjectAssertions)</param>
        /// <param name="expectedUpdates">The object template you want to use for asserting that the updated object has the expected values.
        /// It is possible to use an anonymous object as long as you use the same property names and structure as the object you are asserting.</param>
        /// <param name="originalObject">Optional object that, if provided, will be used to verify that all other properties than the excluded ones
        /// are still matching the object you are asserting. All properties included in the expected updates are (off course) automatically excluded.</param>
        /// <param name="excludePropertiesByTemplate"></param>
        /// <returns></returns>
        public static AndConstraint<ObjectAssertions> BeUpdatedAsExpected(
            this ObjectAssertions assertions,
            object expectedUpdates,
            object originalObject = null,
            object excludePropertiesByTemplate = null)
        {
            return assertions.BeUpdatedAsExpected(
                expectedUpdates,
                originalObject,
                excludePropertiesByTemplate?.GetLeafPropertyNames());
        }

        /// <summary>
        /// Asserts that an object was updated as expected.
        /// To ensure the that only the expected properties were updated, and all other properties have the original values,
        /// you can specify the original object for verification. 
        /// </summary>
        /// <param name="assertions">The extended FluentAssertion object (ObjectAssertions)</param>
        /// <param name="expectedUpdates">The object template you want to use for asserting that the updated object has the expected values.
        /// It is possible to use an anonymous object as long as you use the same property names and structure as the object you are asserting.</param>
        /// <param name="originalObject">Optional object that, if provided, will be used to verify that all other properties than the excluded ones
        /// are still matching the object you are asserting. All properties included in the expected updates are (off course) automatically excluded.</param>
        /// <param name="excludePropertySelectors"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AndConstraint<ObjectAssertions> BeUpdatedAsExpected<T>(
            this ObjectAssertions assertions,
            object expectedUpdates,
            T originalObject,
            params Expression<Func<T, object>>[] excludePropertySelectors)
        {
            return assertions.BeUpdatedAsExpected(
                expectedUpdates,
                originalObject,
                excludePropertySelectors.Select(x => PropertyNameReflectionUtils.GetLeafMemberName(x.Body)).ToList());
        }

        /// <summary>
        /// Asserts that an object was updated as expected.
        /// To ensure the that only the expected properties were updated, and all other properties have the original values,
        /// you can specify the original object for verification. 
        /// </summary>
        /// <param name="assertions">The extended FluentAssertion object (ObjectAssertions)</param>
        /// <param name="expectedUpdates">The object template you want to use for asserting that the updated object has the expected values.
        /// It is possible to use an anonymous object as long as you use the same property names and structure as the object you are asserting.</param>
        /// <param name="originalObject">Optional object that, if provided, will be used to verify that all other properties than the excluded ones
        /// are still matching the object you are asserting. All properties included in the expected updates are (off course) automatically excluded.</param>
        /// <param name="excludePropertyNames">Optional list of property names (full names if leaf properties) to exclude from </param>
        /// <returns>AndConstraint that makes it possible to continue chaining calls using FluentAssertions.</returns>
        public static AndConstraint<ObjectAssertions> BeUpdatedAsExpected(
            this ObjectAssertions assertions,
            object expectedUpdates,
            object originalObject,
            IReadOnlyCollection<string> excludePropertyNames)
        {
            if (originalObject != null)
            {
                var propertiesToExcludeIncludingTheExpectedValuesThatAreAssertedSeparately =
                    excludePropertyNames?.ToList() ?? new List<string>();
                propertiesToExcludeIncludingTheExpectedValuesThatAreAssertedSeparately.AddRange(
                    expectedUpdates.GetLeafPropertyNames());

                return assertions.BeEquivalentTo(
                        originalObject,
                        config => config.Excluding(x => propertiesToExcludeIncludingTheExpectedValuesThatAreAssertedSeparately.ContainsPropertyPath(x.Path)),
                        "the expected updates should be the only changes made to the original object"
                    )
                    .And.BeEquivalentTo(
                        expectedUpdates,
                        config => config.Excluding(x => excludePropertyNames.ContainsPropertyPath(x.Path)));
            }

            return assertions.BeEquivalentTo(
                expectedUpdates,
                config => config.Excluding(x => excludePropertyNames.ContainsPropertyPath(x.Path)));
        }

        /// <summary>
        /// Assembles a collection of property names to exclude from an object comparison.
        /// </summary>
        /// <param name="obj">Extension object that defines the type</param>
        /// <param name="propertySelectors">Specification of the properties to exclude</param>
        /// <returns>A collection of names of the specified properties to exclude.</returns>
        public static IReadOnlyCollection<string> DefineExclusions<T>(
            this T obj,
            params Expression<Func<T, object>>[] propertySelectors)
        {
            return propertySelectors.Select(x => PropertyNameReflectionUtils.GetLeafMemberName(x.Body)).ToList();
        }

        /// <summary>
        /// Gets the name of a member from a lambda expression.
        /// </summary>
        public static string GetMemberName<T>(Expression<Func<T, object>> expression)
        {
            return PropertyNameReflectionUtils.GetLeafMemberName(expression.Body);
        }

        /// <summary>
        /// Uses reflection on the type of the specified object and tries to find the full name of all the public properties.
        /// If the type is a class that has complex types as properties, it will return the full path to the leaf properties.
        /// Example:
        /// A class Person that has a property Address of type Address,
        /// and Address has a property StreetName of type string.
        /// And the Person also has a property Name of type string.
        /// It will return ["Address.StreetName", "Name"]
        /// </summary>
        /// <param name="obj">An object you want to use as a type reference.</param>
        /// <returns>A collection of fully qualified property names for all the public properties of a type.</returns>
        public static IReadOnlyCollection<string> GetLeafPropertyNames(this object obj)
        {
            return GetLeafPropertyNamesForType(obj?.GetType());
        }

        /// <summary>
        /// Uses reflection on the specified type and tries to find the full name of all the public properties.
        /// If the type is a class that has complex types as properties, it will return the full path to the leaf properties.
        /// Example:
        /// A class Person that has a property Address of type Address,
        /// and Address has a property StreetName of type string.
        /// And the Person also has a property Name of type string.
        /// It will return ["Address.StreetName", "Name"]
        /// </summary>
        /// <returns>A collection of fully qualified property names for all the public properties of a type.</returns>
        public static IReadOnlyCollection<string> GetLeafPropertyNamesForType(this Type type)
        {
            if (type == null)
                return Array.Empty<string>();

            var properties = PropertyNameReflectionUtils.GetPublicPropertiesForType(type);
            var leafPropertyNames = new List<string>();

            foreach (var property in properties)
            {
                leafPropertyNames.AddRange(property.GetPropertyNames());
            }

            return leafPropertyNames;
        }
    }
}