using System;
using System.Collections.Generic;

namespace ExCSS
{
    /// <summary>
    /// Expands transition shorthand into property, duration, timing-function, delay.
    /// </summary>
    public sealed class TransitionExpander : IShorthandExpander
    {
        private static readonly HashSet<string> TimingFunctions = new(StringComparer.OrdinalIgnoreCase)
        {
            "linear","ease","ease-in","ease-out","ease-in-out","step-start","step-end"
        };

        public IReadOnlyList<string> ShorthandNames => new[] { PropertyNames.Transition };

        public IReadOnlyList<string> LonghandNames => new[]
        {
            PropertyNames.TransitionProperty,
            PropertyNames.TransitionDuration,
            PropertyNames.TransitionTimingFunction,
            PropertyNames.TransitionDelay
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
                    case Time _:
                        if (!result.ContainsKey(PropertyNames.TransitionDuration))
                            result[PropertyNames.TransitionDuration] = v;
                        else
                            result[PropertyNames.TransitionDelay] = v;
                        break;
                    case KeywordValue k when TimingFunctions.Contains(k.Value):
                        result[PropertyNames.TransitionTimingFunction] = k;
                        break;
                    default:
                        // property name or custom function
                        result[PropertyNames.TransitionProperty] = v;
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
