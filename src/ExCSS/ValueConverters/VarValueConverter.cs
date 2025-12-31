using System;
using System.Collections.Generic;

namespace ExCSS
{
    internal sealed class VarValueConverter : IValueConverter
    {
        public IPropertyValue Convert(IEnumerable<Token> value)
        {
            var function = value.OnlyOrDefault() as FunctionToken;
            if (function == null || !function.Data.Equals(FunctionNames.Var, StringComparison.OrdinalIgnoreCase))
                return null;

            var varValue = ExtractVarValue(function);
            if (varValue == null)
                return null;

            return new VarPropertyValue(varValue, value);
        }

        public IPropertyValue Construct(Property[] properties)
        {
            return properties.Guard<VarPropertyValue>();
        }

        private static VarValue ExtractVarValue(FunctionToken function)
        {
            string variableName = null;
            IStyleValue fallback = null;
            var fallbackTokens = new List<Token>();
            var seenComma = false;
            var pendingDashes = 0;

            foreach (var token in function.ArgumentTokens)
            {
                if (token.Type == TokenType.Whitespace)
                    continue;

                if (variableName == null)
                {
                    if (token.Type == TokenType.Delim && token.Data == "-")
                    {
                        pendingDashes++;
                        continue;
                    }

                    if (token.Type == TokenType.Ident)
                    {
                        var prefix = new string('-', pendingDashes);
                        variableName = prefix + token.Data;
                        pendingDashes = 0;
                        continue;
                    }
                }

                if (token.Type == TokenType.Comma)
                {
                    seenComma = true;
                    continue;
                }

                if (seenComma)
                {
                    fallbackTokens.Add(token);
                }
            }

            if (variableName == null)
                return null;

            if (fallbackTokens.Count > 0)
            {
                fallback = new RawValue(fallbackTokens.ToText());
            }

            return new VarValue(variableName, fallback);
        }

        private sealed class VarPropertyValue : IPropertyValue, ITypedPropertyValue
        {
            private readonly VarValue _value;

            public VarPropertyValue(VarValue value, IEnumerable<Token> tokens)
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
