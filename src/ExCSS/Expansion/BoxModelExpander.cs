using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    /// <summary>
    /// Expands box model shorthand properties (margin, padding, inset).
    /// Handles 1-4 value TRBL (top-right-bottom-left) pattern.
    /// </summary>
    public sealed class BoxModelExpander : IShorthandExpander
    {
        private readonly string _shorthand;
        private readonly string[] _longhands;

        public BoxModelExpander(string shorthand, string[] suffixes = null)
        {
            _shorthand = shorthand;
            var suff = suffixes ?? new[] { "top", "right", "bottom", "left" };
            _longhands = suff.Select(s => $"{shorthand}-{s}").ToArray();
        }

        public IReadOnlyList<string> ShorthandNames => new[] { _shorthand };
        public IReadOnlyList<string> LonghandNames => _longhands;

        public IReadOnlyDictionary<string, IStyleValue> Expand(IStyleValue value)
        {
            var values = ExtractValues(value);
            if (values.Length == 0)
                return new Dictionary<string, IStyleValue>();

            // CSS box model: 1=all, 2=v/h, 3=t/h/b, 4=t/r/b/l
            var expanded = values.Length switch
            {
                1 => new[] { values[0], values[0], values[0], values[0] },
                2 => new[] { values[0], values[1], values[0], values[1] },
                3 => new[] { values[0], values[1], values[2], values[1] },
                _ => new[] { values[0], values[1], values[2], values.Length > 3 ? values[3] : values[1] }
            };

            var result = new Dictionary<string, IStyleValue>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < _longhands.Length && i < expanded.Length; i++)
            {
                result[_longhands[i]] = expanded[i];
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
