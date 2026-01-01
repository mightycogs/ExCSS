using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    public sealed class FlexFlowExpander : IShorthandExpander
    {
        private const string FlexDirection = "flex-direction";
        private const string FlexWrap = "flex-wrap";

        private static readonly HashSet<string> DirectionKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "row", "row-reverse", "column", "column-reverse"
        };

        private static readonly HashSet<string> WrapKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "nowrap", "wrap", "wrap-reverse"
        };

        public IReadOnlyList<string> ShorthandNames => new[] { "flex-flow" };
        public IReadOnlyList<string> LonghandNames => new[] { FlexDirection, FlexWrap };

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
                    string.Equals(keyword, "unset", StringComparison.OrdinalIgnoreCase))
                {
                    result[FlexDirection] = kw;
                    result[FlexWrap] = kw;
                    return result;
                }

                if (DirectionKeywords.Contains(keyword))
                {
                    result[FlexDirection] = kw;
                    result[FlexWrap] = new KeywordValue("nowrap");
                    return result;
                }

                if (WrapKeywords.Contains(keyword))
                {
                    result[FlexDirection] = new KeywordValue("row");
                    result[FlexWrap] = kw;
                    return result;
                }
            }

            var values = ExpanderHelpers.ExtractValues(value);
            if (values.Length == 0)
                return result;

            IStyleValue direction = new KeywordValue("row");
            IStyleValue wrap = new KeywordValue("nowrap");
            bool hasDirection = false;
            bool hasWrap = false;

            foreach (var v in values)
            {
                if (v is KeywordValue kwv)
                {
                    var kwValue = kwv.Value;
                    if (DirectionKeywords.Contains(kwValue))
                    {
                        if (hasDirection)
                            return result;
                        direction = v;
                        hasDirection = true;
                    }
                    else if (WrapKeywords.Contains(kwValue))
                    {
                        if (hasWrap)
                            return result;
                        wrap = v;
                        hasWrap = true;
                    }
                }
            }

            result[FlexDirection] = direction;
            result[FlexWrap] = wrap;
            return result;
        }
    }
}
