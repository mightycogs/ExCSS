using System;
using System.Collections.Generic;

namespace ExCSS
{
    /// <summary>
    /// Expands animation shorthand into its 8 longhands.
    /// </summary>
    public sealed class AnimationExpander : IShorthandExpander
    {
        public IReadOnlyList<string> ShorthandNames => new[] { PropertyNames.Animation };

        public IReadOnlyList<string> LonghandNames => new[]
        {
            PropertyNames.AnimationName,
            PropertyNames.AnimationDuration,
            PropertyNames.AnimationTimingFunction,
            PropertyNames.AnimationDelay,
            PropertyNames.AnimationIterationCount,
            PropertyNames.AnimationDirection,
            PropertyNames.AnimationFillMode,
            PropertyNames.AnimationPlayState
        };

        private static readonly HashSet<string> TimingFunctions = new(StringComparer.OrdinalIgnoreCase)
        {
            "linear","ease","ease-in","ease-out","ease-in-out","step-start","step-end"
        };

        private static readonly HashSet<string> Directions = new(StringComparer.OrdinalIgnoreCase)
        {
            "normal","reverse","alternate","alternate-reverse"
        };

        private static readonly HashSet<string> FillModes = new(StringComparer.OrdinalIgnoreCase)
        {
            "none","forwards","backwards","both"
        };

        private static readonly HashSet<string> PlayStates = new(StringComparer.OrdinalIgnoreCase)
        {
            "running","paused"
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
            foreach (var v in values)
            {
                switch (v)
                {
                    case Time t:
                        if (!result.ContainsKey(PropertyNames.AnimationDuration))
                            result[PropertyNames.AnimationDuration] = t;
                        else
                            result[PropertyNames.AnimationDelay] = t;
                        break;
                    case KeywordValue k when TimingFunctions.Contains(k.Value):
                        result[PropertyNames.AnimationTimingFunction] = k;
                        break;
                    case KeywordValue k when Directions.Contains(k.Value):
                        result[PropertyNames.AnimationDirection] = k;
                        break;
                    case KeywordValue k when FillModes.Contains(k.Value):
                        result[PropertyNames.AnimationFillMode] = k;
                        break;
                    case KeywordValue k when PlayStates.Contains(k.Value):
                        result[PropertyNames.AnimationPlayState] = k;
                        break;
                    case Number _:
                    case RawValue raw when IsNumberKeyword(raw.Value):
                        result[PropertyNames.AnimationIterationCount] = v;
                        break;
                    default:
                        // Treat as name fallback
                        result[PropertyNames.AnimationName] = v;
                        break;
                }
            }

            return result;
        }

        private static bool IsNumberKeyword(string text)
        {
            return string.Equals(text?.Trim(), "infinite", StringComparison.OrdinalIgnoreCase);
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
