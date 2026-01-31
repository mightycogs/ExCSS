using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    internal sealed class AnyValueConverter : IValueConverter
    {
        public IPropertyValue Convert(IEnumerable<Token> value)
        {
            var singleToken = value.OnlyOrDefault();

            if (singleToken is FunctionToken function)
            {
                if (function.Data.Equals(FunctionNames.Var, StringComparison.OrdinalIgnoreCase))
                {
                    var varValue = ExtractVarValue(function);
                    if (varValue != null)
                        return new TypedAnyValue(varValue, value);
                }

                if (function.Data.Equals(FunctionNames.Calc, StringComparison.OrdinalIgnoreCase))
                {
                    var calcValue = ExtractCalcValue(function);
                    if (calcValue != null)
                        return new TypedAnyValue(calcValue, value);
                }

                if (IsGradientFunction(function.Data))
                {
                    var gradientValue = ExtractGradientValue(function);
                    if (gradientValue != null)
                        return new TypedAnyValue(gradientValue, value);
                }
            }

            if (singleToken != null && singleToken.Type == TokenType.Color)
            {
                if (Color.TryFromHex(singleToken.Data, out var color))
                    return new TypedAnyValue(color, value);
            }

            return new AnyValue(value);
        }

        public IPropertyValue Construct(Property[] properties)
        {
            return properties.Guard<AnyValue>();
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

        private static CalcValue ExtractCalcValue(FunctionToken function)
        {
            var tokens = Enumerable.ToList(function.ArgumentTokens);
            var parser = new CalcExpressionParser(tokens);
            var root = parser.Parse();

            if (root == null)
            {
                var expression = function.ArgumentTokens.ToText();
                root = new CalcRawExpression(expression);
            }

            return new CalcValue(root);
        }

        private static bool IsGradientFunction(string name)
        {
            return name.Equals(FunctionNames.LinearGradient, StringComparison.OrdinalIgnoreCase) ||
                   name.Equals(FunctionNames.RadialGradient, StringComparison.OrdinalIgnoreCase) ||
                   name.Equals(FunctionNames.RepeatingLinearGradient, StringComparison.OrdinalIgnoreCase) ||
                   name.Equals(FunctionNames.RepeatingRadialGradient, StringComparison.OrdinalIgnoreCase);
        }

        private static IStyleValue ExtractGradientValue(FunctionToken function)
        {
            var tokens = new List<Token> { function };
            IPropertyValue result = null;

            if (function.Data.Equals(FunctionNames.LinearGradient, StringComparison.OrdinalIgnoreCase) ||
                function.Data.Equals(FunctionNames.RepeatingLinearGradient, StringComparison.OrdinalIgnoreCase))
            {
                result = Converters.LinearGradientConverter.Convert(tokens);
            }
            else if (function.Data.Equals(FunctionNames.RadialGradient, StringComparison.OrdinalIgnoreCase) ||
                     function.Data.Equals(FunctionNames.RepeatingRadialGradient, StringComparison.OrdinalIgnoreCase))
            {
                result = Converters.RadialGradientConverter.Convert(tokens);
            }

            if (result is ITypedPropertyValue typed && typed.GetValue() is IStyleValue sv)
                return sv;

            return null;
        }

        private sealed class TypedAnyValue : IPropertyValue, ITypedPropertyValue
        {
            private readonly IStyleValue _value;

            public TypedAnyValue(IStyleValue value, IEnumerable<Token> tokens)
            {
                _value = value;
                Original = new TokenValue(tokens);
            }

            public string CssText => Original.ToText();

            public TokenValue Original { get; }

            public TokenValue ExtractFor(string name)
            {
                return Original;
            }

            public object GetValue() => _value;
        }

        private sealed class AnyValue : IPropertyValue, ITypedPropertyValue
        {
            public AnyValue(IEnumerable<Token> tokens)
            {
                Original = new TokenValue(tokens);
            }

            public string CssText => Original.ToText();

            public TokenValue Original { get; }

            public TokenValue ExtractFor(string name)
            {
                return Original;
            }

            public object GetValue()
            {
                var text = CssText;
                var items = Original.ToList();
                if (items.Count > 1)
                {
                    var styleValues = new List<IStyleValue>();
                    foreach (var itemTokens in items)
                    {
                        var itemValue = ExtractTypedFromTokens(itemTokens);
                        styleValues.Add(itemValue ?? new RawValue(itemTokens.ToText()));
                    }
                    return new StyleValueList(styleValues);
                }
                if (items.Count == 1)
                {
                    var singleGroup = items[0];
                    var typedValue = ExtractTypedFromTokens(singleGroup);
                    if (typedValue != null)
                        return typedValue;
                    if (singleGroup.Count > 1)
                    {
                        var styleValues = new List<IStyleValue>();
                        foreach (var token in singleGroup)
                        {
                            if (token.Type == TokenType.Whitespace)
                                continue;
                            var tokenValue = ExtractTypedFromSingleToken(token);
                            styleValues.Add(tokenValue ?? new RawValue(token.ToValue()));
                        }
                        if (styleValues.Any(v => !(v is RawValue)))
                            return new StyleValueList(styleValues);
                    }
                }
                return new RawValue(text);
            }

            private static IStyleValue ExtractTypedFromSingleToken(Token token)
            {
                if (token is FunctionToken function)
                {
                    if (function.Data.Equals(FunctionNames.Var, StringComparison.OrdinalIgnoreCase))
                        return ExtractVarValueStatic(function);
                    if (function.Data.Equals(FunctionNames.Calc, StringComparison.OrdinalIgnoreCase))
                        return ExtractCalcValue(function);
                    if (IsGradientFunction(function.Data))
                        return ExtractGradientValue(function);
                    return new FunctionValue(function.Data, function.ArgumentTokens.ToText());
                }
                if (token.Type == TokenType.Color)
                {
                    if (Color.TryFromHex(token.Data, out var color))
                        return color;
                }
                if (token.Type == TokenType.Ident)
                {
                    return new KeywordValue(token.Data);
                }
                if (token.Type == TokenType.Dimension)
                {
                    var unit = token as UnitToken;
                    if (unit != null && float.TryParse(unit.Data, out var numValue))
                    {
                        var lengthUnit = Length.GetUnit(unit.Unit);
                        if (lengthUnit != Length.Unit.None)
                            return new Length(numValue, lengthUnit);
                    }
                }
                if (token.Type == TokenType.Percentage)
                {
                    if (float.TryParse(token.Data, out var pctValue))
                        return new Percent(pctValue);
                }
                if (token.Type == TokenType.Number)
                {
                    if (double.TryParse(token.Data, out var numVal))
                        return new Number(numVal, Number.Unit.Float);
                }
                return null;
            }

            private static IStyleValue ExtractTypedFromTokens(IEnumerable<Token> tokens)
            {
                var function = tokens.FirstOrDefault(t => t is FunctionToken) as FunctionToken;
                if (function != null)
                {
                    if (function.Data.Equals(FunctionNames.Var, StringComparison.OrdinalIgnoreCase))
                        return ExtractVarValueStatic(function);
                    if (function.Data.Equals(FunctionNames.Calc, StringComparison.OrdinalIgnoreCase))
                        return ExtractCalcValue(function);
                    if (IsGradientFunction(function.Data))
                        return ExtractGradientValue(function);
                }
                return null;
            }

            private static VarValue ExtractVarValueStatic(FunctionToken function)
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
        }
    }
}