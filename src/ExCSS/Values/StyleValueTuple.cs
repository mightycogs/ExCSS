using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    /// <summary>
    /// Space-separated tuple of values (e.g., "10px 20px", "1px solid red").
    /// </summary>
    public sealed class StyleValueTuple : IStyleValue, IReadOnlyList<IStyleValue>
    {
        private readonly IStyleValue[] _values;

        public StyleValueTuple(IEnumerable<IStyleValue> values)
        {
            _values = values?.ToArray() ?? Array.Empty<IStyleValue>();
        }

        public StyleValueTuple(params IStyleValue[] values)
        {
            _values = values ?? Array.Empty<IStyleValue>();
        }

        public int Count => _values.Length;
        public IStyleValue this[int index] => index >= 0 && index < _values.Length ? _values[index] : null;

        public string CssText => string.Join(" ", _values.Select(v => v.CssText));
        public StyleValueType Type => StyleValueType.Tuple;

        public override string ToString() => CssText;

        // Convenience accessors for box model (TRBL pattern)
        public IStyleValue Top => _values.Length > 0 ? _values[0] : null;
        public IStyleValue Right => _values.Length > 1 ? _values[1] : (_values.Length > 0 ? _values[0] : null);
        public IStyleValue Bottom => _values.Length > 2 ? _values[2] : (_values.Length > 0 ? _values[0] : null);
        public IStyleValue Left => _values.Length > 3 ? _values[3] : (_values.Length > 1 ? _values[1] : (_values.Length > 0 ? _values[0] : null));

        public IEnumerator<IStyleValue> GetEnumerator() => ((IEnumerable<IStyleValue>)_values).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
