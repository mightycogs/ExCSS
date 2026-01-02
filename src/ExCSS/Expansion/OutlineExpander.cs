using System;
using System.Collections.Generic;

namespace ExCSS
{
    /// <summary>
    /// Expands outline shorthand to outline-color, outline-style, outline-width.
    /// </summary>
    public sealed class OutlineExpander : IShorthandExpander
    {
        private static readonly HashSet<string> OutlineStyles = new(StringComparer.OrdinalIgnoreCase)
        {
            "none", "hidden", "dotted", "dashed", "solid", "double", "groove", "ridge", "inset", "outset"
        };

        public IReadOnlyList<string> ShorthandNames => new[] { PropertyNames.Outline };

        public IReadOnlyList<string> LonghandNames => new[]
        {
            PropertyNames.OutlineColor,
            PropertyNames.OutlineStyle,
            PropertyNames.OutlineWidth
        };

        public IReadOnlyDictionary<string, IStyleValue> Expand(IStyleValue value)
        {
            var result = new Dictionary<string, IStyleValue>(StringComparer.OrdinalIgnoreCase);

            if (value == null)
                return result;

            if (value is KeywordValue globalKw && IsGlobalKeyword(globalKw.Value))
            {
                result[PropertyNames.OutlineColor] = globalKw;
                result[PropertyNames.OutlineStyle] = globalKw;
                result[PropertyNames.OutlineWidth] = globalKw;
                return result;
            }

            var values = ExpanderHelpers.ExtractValues(value);

            foreach (var v in values)
            {
                switch (v)
                {
                    case Color _:
                        result[PropertyNames.OutlineColor] = v;
                        break;
                    case Length _:
                        result[PropertyNames.OutlineWidth] = v;
                        break;
                    case KeywordValue kw when OutlineStyles.Contains(kw.Value):
                        result[PropertyNames.OutlineStyle] = kw;
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
            if (OutlineStyles.Contains(text))
            {
                result[PropertyNames.OutlineStyle] = new KeywordValue(text);
            }
            else if (Length.TryParse(text, out var len))
            {
                result[PropertyNames.OutlineWidth] = len;
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
