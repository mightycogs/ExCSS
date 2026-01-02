using System;
using System.Collections.Generic;

namespace ExCSS
{
    /// <summary>
    /// Expands logical pair shorthands like margin-inline, padding-block (start/end).
    /// </summary>
    public sealed class LogicalPairExpander : IShorthandExpander
    {
        private readonly string _shorthand;
        private readonly string _start;
        private readonly string _end;

        public LogicalPairExpander(string shorthand, string startLonghand, string endLonghand)
        {
            _shorthand = shorthand;
            _start = startLonghand;
            _end = endLonghand;
        }

        public IReadOnlyList<string> ShorthandNames => new[] { _shorthand };

        public IReadOnlyList<string> LonghandNames => new[] { _start, _end };

        public IReadOnlyDictionary<string, IStyleValue> Expand(IStyleValue value)
        {
            var result = new Dictionary<string, IStyleValue>(StringComparer.OrdinalIgnoreCase);

            if (value == null)
                return result;

            if (value is KeywordValue kw && IsGlobalKeyword(kw.Value))
            {
                result[_start] = kw;
                result[_end] = kw;
                return result;
            }

            var values = ExpanderHelpers.ExtractValues(value);
            if (values.Length == 0 || values.Length > 2)
                return result;

            result[_start] = values[0];
            result[_end] = values.Length > 1 ? values[1] : values[0];
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
