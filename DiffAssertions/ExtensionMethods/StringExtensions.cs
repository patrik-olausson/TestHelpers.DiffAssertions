using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace TestHelpers.DiffAssertions
{
    /// <summary>
    /// Extension methods for working with strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Helper mehtod that searches a (JSON) text for all guids that contains dashes and replaces them 
        /// with the specified replacement value in order to make it more usable together diff assertions.
        /// </summary>
        public static string ReplaceGuids(this string value, string replacementValue = "ReplacedGuid")
        {
            return ReplaceMatch(value, "........-....-....-....-............", replacementValue);
        }

        /// <summary>
        /// Helper method that searches a (JSON) text for all dates and 
        /// replaces them with the specified replacement value in order to make it more usable together with diff assertions.
        /// </summary>
        public static string ReplaceJsonFormatedDateTime(this string value, string replacementValue = "ReplacedDateTime")
        {
            return ReplaceMatch(value, @"""....-..-...*""", replacementValue);
        }

        /// <summary>
        /// Helper method that searches a text for a specified value (pattern) and replaces 
        /// all matches with the specified replacement value to make it more usable together with diff assertions.
        /// The value to replace could either be an exact value or an expression.
        /// </summary>
        public static string ReplaceMatch(this string value, string valueToReplace, string replacementValue = "ReplacedValue")
        {
            return new Regex(valueToReplace).Replace(value, replacementValue);
        }

        /// <summary>
        /// Takes any object and tries to serialize it to a JSON string using JsonConvert
        /// and some default settings. Very useful when using diff assertions.
        /// </summary>
        /// <param name="objectToSerialize">The object you want to serialize.</param>
        /// <param name="indented">Specify if you want the JSON to be indented or not. 
        /// Since this is targeted for testing the default value is True.</param>
        /// <returns>A JSON string representation of the provided object.</returns>
        public static string ToJsonString(this object objectToSerialize, bool indented = true)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = indented ? Formatting.Indented : Formatting.None
            };
            return JsonConvert.SerializeObject(
                objectToSerialize,
                settings);
        }

        /// <summary>
        /// Takes a string containing JSON and tries to deserialize it into an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object you want to deserialize the JSON string into</typeparam>
        /// <param name="stringContainingJson"></param>
        /// <returns>The deserialized object.</returns>
        public static T FromJsonString<T>(this string stringContainingJson)
        {
            return JsonConvert.DeserializeObject<T>(stringContainingJson);
        }
    }
}