using System;
using System.Collections.Generic;

namespace ExCSS
{
    public sealed class GridLineExpander : IShorthandExpander
    {
        private readonly string _shorthand;
        private readonly string _startProp;
        private readonly string _endProp;

        public GridLineExpander(string shorthand, string startProp, string endProp)
        {
            _shorthand = shorthand;
            _startProp = startProp;
            _endProp = endProp;
        }

        public IReadOnlyList<string> ShorthandNames => new[] { _shorthand };
        public IReadOnlyList<string> LonghandNames => new[] { _startProp, _endProp };

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
                    string.Equals(keyword, "auto", StringComparison.OrdinalIgnoreCase))
                {
                    result[_startProp] = kw;
                    result[_endProp] = kw;
                    return result;
                }
            }

            var cssText = value.CssText?.Trim() ?? "";
            if (string.IsNullOrEmpty(cssText))
                return result;

            if (cssText.Contains("/"))
            {
                var parts = cssText.Split('/');
                result[_startProp] = new KeywordValue(parts[0].Trim());
                result[_endProp] = parts.Length > 1 ? new KeywordValue(parts[1].Trim()) : new KeywordValue("auto");
            }
            else
            {
                result[_startProp] = value;
                result[_endProp] = new KeywordValue("auto");
            }

            return result;
        }
    }
}
