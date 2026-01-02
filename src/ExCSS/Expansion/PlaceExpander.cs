using System;
using System.Collections.Generic;

namespace ExCSS
{
    public sealed class PlaceExpander : IShorthandExpander
    {
        private readonly string _shorthand;
        private readonly string _first;
        private readonly string _second;

        public PlaceExpander(string shorthand, string firstLonghand, string secondLonghand)
        {
            _shorthand = shorthand;
            _first = firstLonghand;
            _second = secondLonghand;
        }

        public IReadOnlyList<string> ShorthandNames => new[] { _shorthand };

        public IReadOnlyList<string> LonghandNames => new[] { _first, _second };

        public IReadOnlyDictionary<string, IStyleValue> Expand(IStyleValue value)
        {
            var result = new Dictionary<string, IStyleValue>(StringComparer.OrdinalIgnoreCase);

            if (value == null)
                return result;

            if (value is KeywordValue kw && IsGlobalKeyword(kw.Value))
            {
                result[_first] = kw;
                result[_second] = kw;
                return result;
            }

            var values = ExpanderHelpers.ExtractValues(value);
            if (values.Length == 0 || values.Length > 2)
                return result;

            var first = values[0];
            var second = values.Length > 1 ? values[1] : first;

            result[_first] = first;
            result[_second] = second;
            return result;
        }

        private static bool IsGlobalKeyword(string keyword)
        {
            return string.Equals(keyword, "inherit", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(keyword, "initial", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(keyword, "unset", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(keyword, "revert", StringComparison.OrdinalIgnoreCase);
        }
    }
}
