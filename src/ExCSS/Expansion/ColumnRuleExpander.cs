using System;
using System.Collections.Generic;

namespace ExCSS
{
    /// <summary>
    /// Expands column-rule shorthand into color, style, width.
    /// </summary>
    public sealed class ColumnRuleExpander : IShorthandExpander
    {
        private static readonly HashSet<string> RuleStyles = new(StringComparer.OrdinalIgnoreCase)
        {
            "none", "hidden", "dotted", "dashed", "solid", "double", "groove", "ridge", "inset", "outset"
        };

        public IReadOnlyList<string> ShorthandNames => new[] { PropertyNames.ColumnRule };

        public IReadOnlyList<string> LonghandNames => new[]
        {
            PropertyNames.ColumnRuleColor,
            PropertyNames.ColumnRuleStyle,
            PropertyNames.ColumnRuleWidth
        };

        public IReadOnlyDictionary<string, IStyleValue> Expand(IStyleValue value)
        {
            var result = new Dictionary<string, IStyleValue>(StringComparer.OrdinalIgnoreCase);

            if (value == null)
                return result;

            if (value is KeywordValue kw && IsGlobalKeyword(kw.Value))
            {
                foreach (var name in LonghandNames)
                    result[name] = kw;
                return result;
            }

            var values = ExpanderHelpers.ExtractValues(value);

            foreach (var v in values)
            {
                switch (v)
                {
                    case Color _:
                        result[PropertyNames.ColumnRuleColor] = v;
                        break;
                    case Length _:
                        result[PropertyNames.ColumnRuleWidth] = v;
                        break;
                    case KeywordValue k when RuleStyles.Contains(k.Value):
                        result[PropertyNames.ColumnRuleStyle] = k;
                        break;
                    case RawValue raw:
                        ClassifyRawValue(raw, result);
                        break;
                }
            }

            return result;
        }

        private static void ClassifyRawValue(RawValue raw, Dictionary<string, IStyleValue> result)
        {
            var text = raw.Value?.Trim() ?? string.Empty;
            if (RuleStyles.Contains(text))
            {
                result[PropertyNames.ColumnRuleStyle] = new KeywordValue(text);
            }
            else if (Length.TryParse(text, out var len))
            {
                result[PropertyNames.ColumnRuleWidth] = len;
            }
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
