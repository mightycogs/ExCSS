using System;
using System.Collections.Generic;

namespace ExCSS
{
    /// <summary>
    /// Expands border-*- shorthands (top/right/bottom/left) into width, style, color.
    /// </summary>
    public sealed class BorderSideExpander : IShorthandExpander
    {
        private static readonly HashSet<string> BorderStyles = new(StringComparer.OrdinalIgnoreCase)
        {
            "none", "hidden", "dotted", "dashed", "solid", "double", "groove", "ridge", "inset", "outset"
        };

        private readonly string _shorthand;
        private readonly string _width;
        private readonly string _style;
        private readonly string _color;

        public BorderSideExpander(string shorthand, string width, string style, string color)
        {
            _shorthand = shorthand;
            _width = width;
            _style = style;
            _color = color;
        }

        public IReadOnlyList<string> ShorthandNames => new[] { _shorthand };

        public IReadOnlyList<string> LonghandNames => new[] { _width, _style, _color };

        public IReadOnlyDictionary<string, IStyleValue> Expand(IStyleValue value)
        {
            var result = new Dictionary<string, IStyleValue>(StringComparer.OrdinalIgnoreCase);

            if (value == null)
                return result;

            if (value is KeywordValue kw && IsGlobalKeyword(kw.Value))
            {
                result[_width] = kw;
                result[_style] = kw;
                result[_color] = kw;
                return result;
            }

            var values = ExpanderHelpers.ExtractValues(value);

            foreach (var v in values)
            {
                switch (v)
                {
                    case Length _:
                        result[_width] = v;
                        break;
                    case Color _:
                        result[_color] = v;
                        break;
                    case KeywordValue k when BorderStyles.Contains(k.Value):
                        result[_style] = k;
                        break;
                    case RawValue raw:
                        ClassifyRawValue(raw, result);
                        break;
                }
            }

            return result;
        }

        private void ClassifyRawValue(RawValue raw, Dictionary<string, IStyleValue> result)
        {
            var text = raw.Value?.Trim() ?? string.Empty;

            if (BorderStyles.Contains(text))
            {
                result[_style] = new KeywordValue(text);
            }
            else if (Length.TryParse(text, out var len))
            {
                result[_width] = len;
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
