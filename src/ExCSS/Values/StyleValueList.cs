using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    /// <summary>
    /// Comma-separated list of values (e.g., font-family, box-shadow layers).
    /// </summary>
    public sealed class StyleValueList : IStyleValue, IReadOnlyList<IStyleValue>
    {
        private readonly IStyleValue[] _values;

        public StyleValueList(IEnumerable<IStyleValue> values)
        {
            _values = values?.ToArray() ?? Array.Empty<IStyleValue>();
        }

        public StyleValueList(params IStyleValue[] values)
        {
            _values = values ?? Array.Empty<IStyleValue>();
        }

        public int Count => _values.Length;
        public IStyleValue this[int index] => index >= 0 && index < _values.Length ? _values[index] : null;

        public string CssText => string.Join(", ", _values.Select(v => v.CssText));
        public StyleValueType Type => StyleValueType.List;

        public override string ToString() => CssText;

        public IEnumerator<IStyleValue> GetEnumerator() => ((IEnumerable<IStyleValue>)_values).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
