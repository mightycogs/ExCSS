using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    /// <summary>
    /// Expands list-style shorthand to list-style-type, list-style-position, list-style-image.
    /// </summary>
    public sealed class ListStyleExpander : IShorthandExpander
    {
        private static readonly HashSet<string> PositionKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "inside", "outside"
        };

        public IReadOnlyList<string> ShorthandNames => new[] { PropertyNames.ListStyle };

        public IReadOnlyList<string> LonghandNames => new[]
        {
            PropertyNames.ListStyleType,
            PropertyNames.ListStylePosition,
            PropertyNames.ListStyleImage
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

            IStyleValue type = new KeywordValue("disc");
            IStyleValue position = new KeywordValue("outside");
            IStyleValue image = new KeywordValue("none");

            foreach (var v in values)
            {
                switch (v)
                {
                    case KeywordValue k when PositionKeywords.Contains(k.Value):
                        position = k;
                        break;
                    case KeywordValue k when string.Equals(k.Value, "none", StringComparison.OrdinalIgnoreCase):
                        type = k;
                        image = k;
                        break;
                    case UrlValue _:
                        image = v;
                        break;
                    default:
                        // treat as type fallback
                        type = v;
                        break;
                }
            }

            result[PropertyNames.ListStyleType] = type;
            result[PropertyNames.ListStylePosition] = position;
            result[PropertyNames.ListStyleImage] = image;
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
