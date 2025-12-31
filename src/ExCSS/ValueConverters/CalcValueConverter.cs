using System;
using System.Collections.Generic;

namespace ExCSS
{
    internal sealed class CalcValueConverter : IValueConverter
    {
        public IPropertyValue Convert(IEnumerable<Token> value)
        {
            var function = value.OnlyOrDefault() as FunctionToken;
            if (function == null || !function.Data.Equals(FunctionNames.Calc, StringComparison.OrdinalIgnoreCase))
                return null;

            var expression = function.ArgumentTokens.ToText();
            var calcValue = new CalcValue(expression);
            return new CalcPropertyValue(calcValue, value);
        }

        public IPropertyValue Construct(Property[] properties)
        {
            return properties.Guard<CalcPropertyValue>();
        }

        private sealed class CalcPropertyValue : IPropertyValue, ITypedPropertyValue
        {
            private readonly CalcValue _value;

            public CalcPropertyValue(CalcValue value, IEnumerable<Token> tokens)
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
