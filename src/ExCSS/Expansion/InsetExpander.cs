using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    /// <summary>
    /// Expands inset shorthand to top, right, bottom, left.
    /// </summary>
    public sealed class InsetExpander : IShorthandExpander
    {
        private static readonly string[] Longhands = new[] { "top", "right", "bottom", "left" };

        public IReadOnlyList<string> ShorthandNames => new[] { "inset" };
        public IReadOnlyList<string> LonghandNames => Longhands;

        public IReadOnlyDictionary<string, IStyleValue> Expand(IStyleValue value)
        {
            var values = ExpanderHelpers.ExtractValues(value);
            if (values.Length == 0)
                return new Dictionary<string, IStyleValue>();

            var expanded = values.Length switch
            {
                1 => new[] { values[0], values[0], values[0], values[0] },
                2 => new[] { values[0], values[1], values[0], values[1] },
                3 => new[] { values[0], values[1], values[2], values[1] },
                _ => new[] { values[0], values[1], values[2], values.Length > 3 ? values[3] : values[1] }
            };

            var result = new Dictionary<string, IStyleValue>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < Longhands.Length; i++)
            {
                result[Longhands[i]] = expanded[i];
            }
            return result;
        }
    }
}
