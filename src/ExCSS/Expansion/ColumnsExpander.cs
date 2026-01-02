using System;
using System.Collections.Generic;

namespace ExCSS
{
    /// <summary>
    /// Expands columns shorthand into column-width and column-count.
    /// </summary>
    public sealed class ColumnsExpander : IShorthandExpander
    {
        public IReadOnlyList<string> ShorthandNames => new[] { PropertyNames.Columns };

        public IReadOnlyList<string> LonghandNames => new[]
        {
            PropertyNames.ColumnWidth,
            PropertyNames.ColumnCount
        };

        public IReadOnlyDictionary<string, IStyleValue> Expand(IStyleValue value)
        {
            var result = new Dictionary<string, IStyleValue>(StringComparer.OrdinalIgnoreCase);

            if (value == null)
                return result;

            if (value is KeywordValue kw && IsGlobalKeyword(kw.Value))
            {
                result[PropertyNames.ColumnWidth] = kw;
                result[PropertyNames.ColumnCount] = kw;
                return result;
            }

            var values = ExpanderHelpers.ExtractValues(value);
            if (values.Length == 0 || values.Length > 2)
                return result;

            IStyleValue width = null;
            IStyleValue count = null;

            foreach (var v in values)
            {
                switch (v)
                {
                    case Length _:
                        width = v;
                        break;
                    case Number _:
                        count = v;
                        break;
                    case KeywordValue k when string.Equals(k.Value, "auto", StringComparison.OrdinalIgnoreCase):
                        // treat as auto for whichever is unset later
                        if (width == null) width = k;
                        else if (count == null) count = k;
                        break;
                }
            }

            // If only one value provided and we didn't classify, assume it's width unless it's a Number
            if (values.Length == 1 && width == null && count == null)
            {
                if (values[0] is Number)
                    count = values[0];
                else
                    width = values[0];
            }

            result[PropertyNames.ColumnWidth] = width ?? KeywordValue.Auto;
            result[PropertyNames.ColumnCount] = count ?? KeywordValue.Auto;
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
