using System;
using System.Collections.Generic;

namespace ExCSS
{
    /// <summary>
    /// Expands border-inline / border-block into start/end width/style/color.
    /// </summary>
    public sealed class BorderLogicalExpander : IShorthandExpander
    {
        private static readonly HashSet<string> BorderStyles = new(StringComparer.OrdinalIgnoreCase)
        {
            "none", "hidden", "dotted", "dashed", "solid", "double", "groove", "ridge", "inset", "outset"
        };

        private readonly string _shorthand;
        private readonly string _startWidth;
        private readonly string _startStyle;
        private readonly string _startColor;
        private readonly string _endWidth;
        private readonly string _endStyle;
        private readonly string _endColor;

        public BorderLogicalExpander(string shorthand,
            string startWidth, string startStyle, string startColor,
            string endWidth, string endStyle, string endColor)
        {
            _shorthand = shorthand;
            _startWidth = startWidth;
            _startStyle = startStyle;
            _startColor = startColor;
            _endWidth = endWidth;
            _endStyle = endStyle;
            _endColor = endColor;
        }

        public IReadOnlyList<string> ShorthandNames => new[] { _shorthand };

        public IReadOnlyList<string> LonghandNames => new[]
        {
            _startWidth, _startStyle, _startColor,
            _endWidth, _endStyle, _endColor
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

            IStyleValue width = null;
            IStyleValue style = null;
            IStyleValue color = null;

            foreach (var v in values)
            {
                switch (v)
                {
                    case Length _:
                        width = v;
                        break;
                    case Color _:
                        color = v;
                        break;
                    case KeywordValue k when BorderStyles.Contains(k.Value):
                        style = k;
                        break;
                    case RawValue raw:
                        ClassifyRawValue(raw, ref width, ref style);
                        break;
                }
            }

            if (width != null)
            {
                result[_startWidth] = width;
                result[_endWidth] = width;
            }
            if (style != null)
            {
                result[_startStyle] = style;
                result[_endStyle] = style;
            }
            if (color != null)
            {
                result[_startColor] = color;
                result[_endColor] = color;
            }

            return result;
        }

        private static void ClassifyRawValue(RawValue raw, ref IStyleValue width, ref IStyleValue style)
        {
            var text = raw.Value?.Trim() ?? string.Empty;
            if (BorderStyles.Contains(text))
            {
                style = new KeywordValue(text);
            }
            else if (Length.TryParse(text, out var len))
            {
                width = len;
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
