using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExCSS.Tests.Helpers
{
    /// <summary>
    /// Helper for detecting silent parse failures in CSS parsing.
    /// RawValue with IsParseFailure=true indicates the parser couldn't
    /// create a typed value and fell back to raw string.
    /// </summary>
    public static class ParseFailureDetector
    {
        /// <summary>
        /// Asserts that a TypedValue is not a parse failure.
        /// Use this to ensure CSS was properly parsed into typed values.
        /// </summary>
        public static void AssertNoParseFailure(IStyleValue value, string context = null)
        {
            if (value is RawValue raw && raw.IsParseFailure)
            {
                var msg = $"Silent parse failure detected: '{raw.Value}'";
                if (context != null)
                    msg += $" (context: {context})";
                Assert.Fail(msg);
            }

            // Check nested values in lists
            if (value is StyleValueList list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    AssertNoParseFailure(list[i], $"{context}[{i}]");
                }
            }

            if (value is StyleValueTuple tuple)
            {
                for (int i = 0; i < tuple.Count; i++)
                {
                    AssertNoParseFailure(tuple[i], $"{context}[{i}]");
                }
            }
        }

        /// <summary>
        /// Asserts that a property's TypedValue is not a parse failure.
        /// </summary>
        public static void AssertNoParseFailure(IProperty property)
        {
            var typedValue = property.TypedValue;
            if (typedValue == null)
            {
                Assert.Fail($"Property '{property.Name}' has null TypedValue");
                return;
            }

            AssertNoParseFailure(typedValue, property.Name);
        }

        /// <summary>
        /// Asserts that none of the properties in a style have parse failures.
        /// </summary>
        public static void AssertNoParseFailures(StyleDeclaration style)
        {
            foreach (var prop in style)
            {
                if (string.IsNullOrEmpty(prop.Value))
                    continue;

                AssertNoParseFailure(prop);
            }
        }

        /// <summary>
        /// Finds all parse failures in a stylesheet.
        /// Returns list of (selector, property, raw value) tuples.
        /// </summary>
        public static List<(string Selector, string Property, string RawValue)> FindParseFailures(Stylesheet stylesheet)
        {
            var failures = new List<(string, string, string)>();

            foreach (var rule in stylesheet.StyleRules)
            {
                foreach (var prop in rule.Style)
                {
                    if (string.IsNullOrEmpty(prop.Value))
                        continue;

                    var rawFailures = FindRawFailures(prop.TypedValue);
                    foreach (var raw in rawFailures)
                    {
                        failures.Add((rule.SelectorText, prop.Name, raw));
                    }
                }
            }

            return failures;
        }

        private static List<string> FindRawFailures(IStyleValue value)
        {
            var failures = new List<string>();

            if (value is RawValue raw && raw.IsParseFailure)
            {
                failures.Add(raw.Value);
            }
            else if (value is StyleValueList list)
            {
                foreach (var item in list)
                {
                    failures.AddRange(FindRawFailures(item));
                }
            }
            else if (value is StyleValueTuple tuple)
            {
                foreach (var item in tuple)
                {
                    failures.AddRange(FindRawFailures(item));
                }
            }

            return failures;
        }

        /// <summary>
        /// Asserts that TypedValue is of a specific type (not RawValue fallback).
        /// </summary>
        public static T AssertTypedValue<T>(IProperty property) where T : class, IStyleValue
        {
            var typedValue = property.TypedValue;
            Assert.NotNull(typedValue);

            if (typedValue is RawValue raw)
            {
                Assert.Fail($"Property '{property.Name}' fell back to RawValue: '{raw.Value}' (IsParseFailure={raw.IsParseFailure}). Expected: {typeof(T).Name}");
            }

            var result = typedValue as T;
            Assert.NotNull(result);
            return result;
        }
    }
}
