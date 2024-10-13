using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TestHelpers.DiffAssertions.Utils
{
    /// <summary>
    /// Helper methods for extracting member name using reflection.
    /// </summary>
    internal static class PropertyNameReflectionUtils
    {
        /// <summary>
        /// Gets the full member name of the leaf member in the expression.
        /// </summary>
        /// <param name="expression">The normal use case should be to use the body of a MemberExpression.</param>
        /// <remarks>
        /// It is possible to specify a member that is inside a collection by using a normal indexer[0] syntax but
        /// also by using Linq methods ElementAt(0) or First(). The result will be a string that is useful to
        /// use with FluentAssertions, like: CollectionProperty[0].MemberProperty
        /// </remarks>
        internal static string GetLeafMemberName(Expression expression)
        {
            switch (expression)
            {
                case MemberExpression memberExpression:
                {
                    var parentName = GetLeafMemberName(memberExpression.Expression);
                    var memberName = memberExpression.Member.Name;

                    return parentName == null ? memberName : $"{parentName}.{memberName}";
                }
                case UnaryExpression unaryExpression:
                {
                    return GetLeafMemberName(unaryExpression.Operand);
                }
                case MethodCallExpression methodCallExpression:
                {
                    if (methodCallExpression.Object == null)
                        return GetIndexerString(); 
                    
                    var parentName = GetLeafMemberName(methodCallExpression.Object);
                    var indexerString = GetIndexerString();
                    return $"{parentName}{indexerString}";

                    string GetIndexerString()
                    {
                        
                        if (methodCallExpression.Object == null)
                        {
                            var arg = methodCallExpression.Arguments.FirstOrDefault();
                            if (arg?.NodeType == ExpressionType.MemberAccess)
                            {
                                var memberExpression = (MemberExpression) arg;
                                if (methodCallExpression.Arguments.Count > 1)
                                {
                                    
                                    return $"{memberExpression.Member.Name}[{methodCallExpression.Arguments.ElementAt(1)}]";
                                }
                                
                                return $"{memberExpression.Member.Name}[0]";
                            }
                            
                            throw new InvalidOperationException("Unable to determine indexer string. Use indexer to access object in collection (or Linq ElementAt() or First()).");
                        }
                        
                        var index = methodCallExpression.Arguments.FirstOrDefault();
                        if (index == null)
                            return "[0]";

                        return $"[{index}]";
                    }
                }
                default:
                    return null;
            }
        }

        /// <summary>
        /// Recursive method to get all public property names of a type.
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        internal static IEnumerable<string> GetPropertyNames(this PropertyInfo propertyInfo, string parent = null)
        {
            var propertyType = propertyInfo.PropertyType;
            if (propertyType == typeof(string) || propertyType.IsValueType)
            {
                yield return GetFullName(propertyInfo.Name);
            }
            else if (IsCollection(propertyType))
            {
                Type collectionType = null;
                if (propertyType.IsGenericType)
                {
                    var genericArguments = propertyType.GetGenericArguments();
                    collectionType = genericArguments.Last();
                }
                else if (propertyType.IsArray)
                {
                    collectionType = propertyType.GetElementType();
                }

                if (collectionType == null)
                {
                    yield return GetFullName(propertyInfo.Name);
                }
                else
                {
                    var childProps = collectionType.GetPublicPropertiesForType();
                    if (childProps.Count == 0)
                    {
                        yield return GetFullName(propertyInfo.Name);
                    }
                    else
                    {
                        foreach (var childProp in childProps)
                        {
                            foreach (var name in GetPropertyNames(childProp, propertyInfo.Name))
                            {
                                yield return GetFullName(name);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var childProp in propertyType.GetPublicPropertiesForType())
                {
                    foreach (var name in GetPropertyNames(childProp, propertyInfo.Name))
                    {
                        yield return GetFullName(name);
                    }
                }
            }

            string GetFullName(string name)
            {
                if (string.IsNullOrWhiteSpace(parent))
                    return name;

                return $"{parent}.{name}";
            }

            bool IsCollection(Type type)
            {
                return typeof(IEnumerable).IsAssignableFrom(type);
            }
        }
        
        /// <summary>
        /// Simple extension method to get all public properties for a type.
        /// </summary>
        internal static IReadOnlyCollection<PropertyInfo> GetPublicPropertiesForType(this Type type)
        {
            return type.GetProperties();
        }

        /// <summary>
        /// Simple extension method that makes it easier to check if a collection of property paths contains a specific path
        /// while also handling null.
        /// </summary>
        internal static bool ContainsPropertyPath(this IReadOnlyCollection<string> propertyNamePaths, string path)
        {
            if (propertyNamePaths == null) return false;

            return propertyNamePaths.Contains(path);
        }
    }
}