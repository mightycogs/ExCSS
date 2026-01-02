using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    /// <summary>
    /// Simplified font shorthand expander (style, variant, weight, stretch, size, line-height, family).
    /// </summary>
    public sealed class FontExpander : IShorthandExpander
    {
        private static readonly HashSet<string> StyleKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "normal","italic","oblique"
        };

        private static readonly HashSet<string> VariantKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "normal","small-caps"
        };

        private static readonly HashSet<string> WeightKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "normal","bold","bolder","lighter","100","200","300","400","500","600","700","800","900"
        };

        private static readonly HashSet<string> StretchKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "normal","ultra-condensed","extra-condensed","condensed","semi-condensed",
            "semi-expanded","expanded","extra-expanded","ultra-expanded"
        };

        public IReadOnlyList<string> ShorthandNames => new[] { PropertyNames.Font };

        public IReadOnlyList<string> LonghandNames => new[]
        {
            PropertyNames.FontStyle,
            PropertyNames.FontVariant,
            PropertyNames.FontWeight,
            PropertyNames.FontStretch,
            PropertyNames.FontSize,
            PropertyNames.LineHeight,
            PropertyNames.FontFamily
        };

        public IReadOnlyDictionary<string, IStyleValue> Expand(IStyleValue value)
        {
            var result = new Dictionary<string, IStyleValue>(StringComparer.OrdinalIgnoreCase);
            if (value == null) return result;

            if (value is KeywordValue kw && IsGlobalKeyword(kw.Value))
            {
                foreach (var name in LonghandNames)
                    result[name] = kw;
                return result;
            }

            var values = ExpanderHelpers.ExtractValues(value);
            var beforeSize = new List<IStyleValue>();
            var afterSlash = new List<IStyleValue>();
            bool seenSlash = false;
            bool sizeCaptured = false;

            foreach (var v in values)
            {
                if (v.CssText.Contains("/"))
                {
                    seenSlash = true;
                    continue;
                }

                if (!sizeCaptured && (v is Length || v is KeywordValue k && string.Equals(k.Value, "small", StringComparison.OrdinalIgnoreCase)))
                {
                    result[PropertyNames.FontSize] = v;
                    sizeCaptured = true;
                    if (seenSlash) continue;
                    continue;
                }

                if (seenSlash && sizeCaptured)
                {
                    afterSlash.Add(v);
                    continue;
                }

                if (!sizeCaptured)
                    beforeSize.Add(v);
                else
                    afterSlash.Add(v); // if something unexpected after size but before slash, treat as afterSlash
            }

            // Parse pre-size keywords
            foreach (var v in beforeSize)
            {
                switch (v)
                {
                    case KeywordValue k when StyleKeywords.Contains(k.Value):
                        result[PropertyNames.FontStyle] = k;
                        break;
                    case KeywordValue k when VariantKeywords.Contains(k.Value):
                        result[PropertyNames.FontVariant] = k;
                        break;
                    case KeywordValue k when WeightKeywords.Contains(k.Value):
                        result[PropertyNames.FontWeight] = k;
                        break;
                    case KeywordValue k when StretchKeywords.Contains(k.Value):
                        result[PropertyNames.FontStretch] = k;
                        break;
                    default:
                        result[PropertyNames.FontFamily] = v;
                        break;
                }
            }

            // Handle line-height and family from afterSlash
            if (afterSlash.Count > 0)
            {
                if (seenSlash)
                {
                    var lineHeightToken = afterSlash.Find(t => t.CssText != "/");
                    if (lineHeightToken != null && !result.ContainsKey(PropertyNames.LineHeight))
                        result[PropertyNames.LineHeight] = lineHeightToken;

                    var familyTokens = new List<IStyleValue>();
                    bool consumedLineHeight = false;
                    foreach (var t in afterSlash)
                    {
                        if (t.CssText == "/") continue;
                        if (!consumedLineHeight && lineHeightToken != null && t == lineHeightToken)
                        {
                            consumedLineHeight = true;
                            continue;
                        }
                        familyTokens.Add(t);
                    }

                    if (familyTokens.Count > 0)
                        result[PropertyNames.FontFamily] = familyTokens.Count == 1 ? familyTokens[0] : new StyleValueTuple(familyTokens);
                }
                else
                {
                    // No slash: everything after size is family
                    result[PropertyNames.FontFamily] = afterSlash.Count == 1 ? afterSlash[0] : new StyleValueTuple(afterSlash);
                }
            }

            // If line-height not provided after slash, default to "normal" when slash present
            if (seenSlash && !result.ContainsKey(PropertyNames.LineHeight))
                result[PropertyNames.LineHeight] = new KeywordValue("normal");

            // Ensure font-family is set if nothing parsed yet and tokens before size exist
            if (!result.ContainsKey(PropertyNames.FontFamily) && beforeSize.Count > 0)
                result[PropertyNames.FontFamily] = beforeSize.Count == 1 ? beforeSize[0] : new StyleValueTuple(beforeSize);

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
