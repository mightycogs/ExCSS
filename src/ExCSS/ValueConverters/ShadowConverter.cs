using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ExCSS
{
    using static Converters;

    internal sealed class ShadowConverter : IValueConverter
    {
        public static readonly ShadowConverter Instance = new();

        public IPropertyValue Convert(IEnumerable<Token> value)
        {
            var tokens = value as IList<Token> ?? value.ToArray();
            var items = tokens.ToItems();

            bool isInset = false;
            var lengths = new List<Length>();
            Color? color = null;
            bool colorExplicit = false;

            foreach (var item in items)
            {
                if (TryParseInset(item, ref isInset)) continue;
                if (TryParseLengths(item, lengths)) continue;
                if (TryParseColor(item, ref color))
                {
                    colorExplicit = true;
                    continue;
                }
                return null;
            }

            if (lengths.Count < 2 || lengths.Count > 4)
                return null;

            var shadow = new Shadow(
                isInset,
                lengths[0],
                lengths[1],
                lengths.Count > 2 ? lengths[2] : Length.Zero,
                lengths.Count > 3 ? lengths[3] : Length.Zero,
                color ?? Color.Black
            );

            return new ShadowValue(shadow, tokens, colorExplicit, lengths.Count);
        }

        public IPropertyValue Construct(Property[] properties)
        {
            return properties.Guard<ShadowValue>();
        }

        private static bool TryParseInset(List<Token> item, ref bool isInset)
        {
            if (item.Count == 1 && item[0] is { Type: TokenType.Ident } ident)
            {
                if (ident.Data.Equals(Keywords.Inset, System.StringComparison.OrdinalIgnoreCase))
                {
                    isInset = true;
                    return true;
                }
            }
            return false;
        }

        private static bool TryParseLengths(List<Token> item, List<Length> lengths)
        {
            var lengthResult = LengthConverter.Convert(item);
            if (lengthResult is ITypedPropertyValue typed && typed.GetValue() is Length len)
            {
                lengths.Add(len);
                return true;
            }

            if (item.Count == 1 && item[0] is NumberToken num && num.Value == 0)
            {
                lengths.Add(Length.Zero);
                return true;
            }

            return false;
        }

        private static bool TryParseColor(List<Token> item, ref Color? color)
        {
            var colorResult = ColorConverter.Convert(item);
            if (colorResult is ITypedPropertyValue typed && typed.GetValue() is Color col)
            {
                color = col;
                return true;
            }
            return false;
        }

        private sealed class ShadowValue : IPropertyValue, ITypedPropertyValue
        {
            private readonly Shadow _shadow;
            private readonly bool _colorExplicit;
            private readonly int _lengthCount;

            public ShadowValue(Shadow shadow, IEnumerable<Token> tokens, bool colorExplicit, int lengthCount)
            {
                _shadow = shadow;
                _colorExplicit = colorExplicit;
                _lengthCount = lengthCount;
                Original = new TokenValue(tokens);
            }

            public string CssText
            {
                get
                {
                    var inv = CultureInfo.InvariantCulture;
                    var parts = new List<string>();
                    if (_shadow.IsInset) parts.Add(Keywords.Inset);
                    parts.Add(_shadow.OffsetX.ToString(null, inv));
                    parts.Add(_shadow.OffsetY.ToString(null, inv));
                    if (_lengthCount >= 3)
                        parts.Add(_shadow.BlurRadius.ToString(null, inv));
                    if (_lengthCount >= 4)
                        parts.Add(_shadow.SpreadRadius.ToString(null, inv));
                    if (_colorExplicit)
                        parts.Add(_shadow.Color.ToString(null, inv));
                    return string.Join(" ", parts);
                }
            }

            public TokenValue Original { get; }

            public TokenValue ExtractFor(string name) => Original;

            public object GetValue() => _shadow;
        }
    }
}
