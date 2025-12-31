using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    /// <summary>
    /// Expands border shorthand to border-width, border-style, border-color.
    /// </summary>
    public sealed class BorderExpander : IShorthandExpander
    {
        private static readonly HashSet<string> BorderStyles = new(StringComparer.OrdinalIgnoreCase)
        {
            "none", "hidden", "dotted", "dashed", "solid", "double", "groove", "ridge", "inset", "outset"
        };

        public IReadOnlyList<string> ShorthandNames => new[] { "border" };

        public IReadOnlyList<string> LonghandNames => new[]
        {
            "border-width", "border-style", "border-color"
        };

        public IReadOnlyDictionary<string, IStyleValue> Expand(IStyleValue value)
        {
            var result = new Dictionary<string, IStyleValue>(StringComparer.OrdinalIgnoreCase);
            var values = ExtractValues(value);

            foreach (var v in values)
            {
                switch (v)
                {
                    case Length _:
                        result["border-width"] = v;
                        break;
                    case Color _:
                        result["border-color"] = v;
                        break;
                    case KeywordValue kw when BorderStyles.Contains(kw.Value):
                        result["border-style"] = v;
                        break;
                    case VarValue _:
                        // var() - assume color (most common use case)
                        if (!result.ContainsKey("border-color"))
                            result["border-color"] = v;
                        break;
                    case RawValue raw:
                        // Try to determine type from raw value
                        ClassifyRawValue(raw, result);
                        break;
                }
            }

            return result;
        }

        private static void ClassifyRawValue(RawValue raw, Dictionary<string, IStyleValue> result)
        {
            var text = raw.Value?.Trim() ?? string.Empty;

            if (BorderStyles.Contains(text))
            {
                result["border-style"] = new KeywordValue(text);
            }
            else if (Length.TryParse(text, out var len))
            {
                result["border-width"] = len;
            }
            // Otherwise could be a color name or other value
        }

        private static IStyleValue[] ExtractValues(IStyleValue value)
        {
            return value switch
            {
                IReadOnlyList<IStyleValue> list => list.ToArray(),
                null => Array.Empty<IStyleValue>(),
                _ => new[] { value }
            };
        }
    }
}
