using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    /// <summary>
    /// Expands border-radius shorthand to corner-specific longhands.
    /// </summary>
    public sealed class BorderRadiusExpander : IShorthandExpander
    {
        private static readonly string[] Corners = new[]
        {
            "border-top-left-radius",
            "border-top-right-radius",
            "border-bottom-right-radius",
            "border-bottom-left-radius"
        };

        public IReadOnlyList<string> ShorthandNames => new[] { "border-radius" };
        public IReadOnlyList<string> LonghandNames => Corners;

        public IReadOnlyDictionary<string, IStyleValue> Expand(IStyleValue value)
        {
            var values = ExtractValues(value);
            if (values.Length == 0)
                return new Dictionary<string, IStyleValue>();

            // border-radius follows same TRBL pattern for corners
            var expanded = values.Length switch
            {
                1 => new[] { values[0], values[0], values[0], values[0] },
                2 => new[] { values[0], values[1], values[0], values[1] },
                3 => new[] { values[0], values[1], values[2], values[1] },
                _ => new[] { values[0], values[1], values[2], values.Length > 3 ? values[3] : values[1] }
            };

            var result = new Dictionary<string, IStyleValue>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < Corners.Length; i++)
            {
                result[Corners[i]] = expanded[i];
            }
            return result;
        }

        private static IStyleValue[] ExtractValues(IStyleValue value)
        {
            return value switch
            {
                StyleValueTuple tuple => tuple.ToArray(),
                IReadOnlyList<IStyleValue> list => list.ToArray(),
                null => Array.Empty<IStyleValue>(),
                _ => new[] { value }
            };
        }
    }
}
