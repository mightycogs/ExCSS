using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    internal sealed class AspectRatioConverter : IValueConverter
    {
        public static readonly AspectRatioConverter Instance = new();

        public IPropertyValue Convert(IEnumerable<Token> value)
        {
            var tokenList = value.Where(t => t.Type != TokenType.Whitespace).ToArray();
            if (tokenList.Length == 0) return null;

            // Check for 'auto' keyword
            if (tokenList.Length == 1 && tokenList[0].Type == TokenType.Ident &&
                tokenList[0].Data.Equals(Keywords.Auto, System.StringComparison.OrdinalIgnoreCase))
            {
                return new AspectRatioValue(AspectRatio.Auto, value);
            }

            // Try parsing: single number OR number / number
            if (tokenList.Length == 1 && tokenList[0].Type == TokenType.Number)
            {
                // Single number like "1.5"
                if (float.TryParse(tokenList[0].Data, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out float ratio) && ratio > 0)
                {
                    return new AspectRatioValue(new AspectRatio(ratio), value);
                }
            }

            if (tokenList.Length == 3 &&
                tokenList[0].Type == TokenType.Number &&
                tokenList[1].Type == TokenType.Delim && tokenList[1].Data == "/" &&
                tokenList[2].Type == TokenType.Number)
            {
                // Format: "16 / 9"
                if (float.TryParse(tokenList[0].Data, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out float width) && width > 0 &&
                    float.TryParse(tokenList[2].Data, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out float height) && height > 0)
                {
                    return new AspectRatioValue(new AspectRatio(width, height), value);
                }
            }

            return null;
        }

        public IPropertyValue Construct(Property[] properties)
        {
            return properties.Guard<AspectRatioValue>();
        }

        private sealed class AspectRatioValue : IPropertyValue, ITypedPropertyValue
        {
            private readonly AspectRatio _value;

            public AspectRatioValue(AspectRatio value, IEnumerable<Token> tokens)
            {
                _value = value;
                Original = new TokenValue(tokens);
            }

            public string CssText => _value.CssText;
            public TokenValue Original { get; }
            public TokenValue ExtractFor(string name) => Original;
            public object GetValue() => _value;
        }
    }
}
