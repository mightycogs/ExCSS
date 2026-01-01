using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    public sealed class GapExpander : IShorthandExpander
    {
        private const string RowGap = "row-gap";
        private const string ColumnGap = "column-gap";

        public IReadOnlyList<string> ShorthandNames => new[] { "gap" };
        public IReadOnlyList<string> LonghandNames => new[] { RowGap, ColumnGap };

        public IReadOnlyDictionary<string, IStyleValue> Expand(IStyleValue value)
        {
            var result = new Dictionary<string, IStyleValue>(StringComparer.OrdinalIgnoreCase);

            if (value == null)
                return result;

            if (value is KeywordValue kw)
            {
                var keyword = kw.Value;
                if (string.Equals(keyword, "inherit", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(keyword, "initial", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(keyword, "unset", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(keyword, "normal", StringComparison.OrdinalIgnoreCase))
                {
                    result[RowGap] = kw;
                    result[ColumnGap] = kw;
                    return result;
                }
            }

            var values = ExpanderHelpers.ExtractValues(value);
            if (values.Length == 0 || values.Length > 2)
                return result;

            result[RowGap] = values[0];
            result[ColumnGap] = values.Length > 1 ? values[1] : values[0];
            return result;
        }
    }
}
