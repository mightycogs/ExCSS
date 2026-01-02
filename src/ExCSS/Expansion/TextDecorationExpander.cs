using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    /// <summary>
    /// Expands text-decoration shorthand into line, color, style, thickness.
    /// </summary>
    public sealed class TextDecorationExpander : IShorthandExpander
    {
        private static readonly HashSet<string> LineKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "none", "underline", "overline", "line-through", "blink"
        };

        private static readonly HashSet<string> StyleKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "solid", "double", "dotted", "dashed", "wavy"
        };

        private static readonly HashSet<string> ThicknessKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "auto", "from-font"
        };

        public IReadOnlyList<string> ShorthandNames => new[] { PropertyNames.TextDecoration };

        public IReadOnlyList<string> LonghandNames => new[]
        {
            PropertyNames.TextDecorationLine,
            PropertyNames.TextDecorationColor,
            PropertyNames.TextDecorationStyle,
            PropertyNames.TextDecorationThickness
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
                    case KeywordValue k when LineKeywords.Contains(k.Value):
                        result[PropertyNames.TextDecorationLine] = k;
                        break;
                    case KeywordValue k when StyleKeywords.Contains(k.Value):
                        result[PropertyNames.TextDecorationStyle] = k;
                        break;
                    case KeywordValue k when ThicknessKeywords.Contains(k.Value):
                        result[PropertyNames.TextDecorationThickness] = k;
                        break;
                    case Color _:
                        result[PropertyNames.TextDecorationColor] = v;
                        break;
                    case Length _:
                        result[PropertyNames.TextDecorationThickness] = v;
                        break;
                }
            }

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
