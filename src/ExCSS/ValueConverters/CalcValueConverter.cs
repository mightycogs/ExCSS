using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ExCSS
{
    internal sealed class CalcValueConverter : IValueConverter
    {
        public IPropertyValue Convert(IEnumerable<Token> value)
        {
            var function = value.OnlyOrDefault() as FunctionToken;
            if (function == null || !function.Data.Equals(FunctionNames.Calc, StringComparison.OrdinalIgnoreCase))
                return null;

            var tokens = Enumerable.ToList(function.ArgumentTokens);
            var parser = new CalcExpressionParser(tokens);
            var root = parser.Parse();

            if (root == null)
            {
                var expression = function.ArgumentTokens.ToText();
                root = new CalcRawExpression(expression);
            }

            var calcValue = new CalcValue(root);
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

    internal sealed class CalcExpressionParser
    {
        private readonly List<Token> _tokens;
        private int _position;

        public CalcExpressionParser(List<Token> tokens)
        {
            _tokens = tokens;
            _position = 0;
        }

        public ICalcExpression Parse()
        {
            SkipWhitespace();
            if (_position >= _tokens.Count)
                return null;

            var result = ParseAdditive();
            SkipWhitespace();
            return result;
        }

        private ICalcExpression ParseAdditive()
        {
            var left = ParseMultiplicative();
            if (left == null)
                return null;

            while (true)
            {
                SkipWhitespace();
                if (_position >= _tokens.Count)
                    break;

                var token = _tokens[_position];
                CalcOperator? op = null;

                if (token.Type == TokenType.Delim)
                {
                    if (token.Data == "+")
                        op = CalcOperator.Add;
                    else if (token.Data == "-")
                        op = CalcOperator.Subtract;
                }

                if (op == null)
                    break;

                _position++;
                SkipWhitespace();

                var right = ParseMultiplicative();
                if (right == null)
                    return null;

                left = new CalcBinaryExpression(left, op.Value, right);
            }

            return left;
        }

        private ICalcExpression ParseMultiplicative()
        {
            var left = ParseUnary();
            if (left == null)
                return null;

            while (true)
            {
                SkipWhitespace();
                if (_position >= _tokens.Count)
                    break;

                var token = _tokens[_position];
                CalcOperator? op = null;

                if (token.Type == TokenType.Delim)
                {
                    if (token.Data == "*")
                        op = CalcOperator.Multiply;
                    else if (token.Data == "/")
                        op = CalcOperator.Divide;
                }

                if (op == null)
                    break;

                _position++;
                SkipWhitespace();

                var right = ParseUnary();
                if (right == null)
                    return null;

                left = new CalcBinaryExpression(left, op.Value, right);
            }

            return left;
        }

        private ICalcExpression ParseUnary()
        {
            SkipWhitespace();
            if (_position >= _tokens.Count)
                return null;

            var token = _tokens[_position];

            if (token.Type == TokenType.Delim && token.Data == "-")
            {
                _position++;
                SkipWhitespace();
                var operand = ParseAtom();
                if (operand == null)
                    return null;

                var negativeOne = new CalcLiteralExpression(new Number(-1d, Number.Unit.Integer));
                return new CalcBinaryExpression(negativeOne, CalcOperator.Multiply, operand);
            }

            if (token.Type == TokenType.Delim && token.Data == "+")
            {
                _position++;
                SkipWhitespace();
            }

            return ParseAtom();
        }

        private ICalcExpression ParseAtom()
        {
            SkipWhitespace();
            if (_position >= _tokens.Count)
                return null;

            var token = _tokens[_position];

            if (token.Type == TokenType.RoundBracketOpen)
            {
                _position++;
                SkipWhitespace();
                var inner = ParseAdditive();
                if (inner == null)
                    return null;
                SkipWhitespace();
                if (_position < _tokens.Count && _tokens[_position].Type == TokenType.RoundBracketClose)
                    _position++;
                return new CalcGroupExpression(inner);
            }

            if (token.Type == TokenType.Function)
            {
                var func = token as FunctionToken;
                if (func != null && func.Data.Equals(FunctionNames.Var, StringComparison.OrdinalIgnoreCase))
                {
                    _position++;
                    var varValue = ExtractVarValue(func);
                    if (varValue != null)
                        return new CalcVarExpression(varValue);
                    return null;
                }
                _position++;
                return new CalcLiteralExpression(new RawValue(func?.ToValue() ?? token.ToValue()));
            }

            if (token.Type == TokenType.Number)
            {
                _position++;
                var numToken = token as NumberToken;
                if (numToken != null)
                {
                    var unit = numToken.IsInteger ? Number.Unit.Integer : Number.Unit.Float;
                    return new CalcLiteralExpression(new Number(numToken.Value, unit));
                }
                return new CalcLiteralExpression(new RawValue(token.Data));
            }

            if (token.Type == TokenType.Percentage)
            {
                _position++;
                var unitToken = token as UnitToken;
                if (unitToken != null)
                {
                    return new CalcLiteralExpression(new Percent(unitToken.Value));
                }
                return new CalcLiteralExpression(new RawValue(token.ToValue()));
            }

            if (token.Type == TokenType.Dimension)
            {
                _position++;
                var unitToken = token as UnitToken;
                if (unitToken != null)
                {
                    var lengthUnit = Length.GetUnit(unitToken.Unit.ToLowerInvariant());
                    if (lengthUnit != Length.Unit.None)
                    {
                        return new CalcLiteralExpression(new Length(unitToken.Value, lengthUnit));
                    }
                    return new CalcLiteralExpression(new RawValue(token.ToValue()));
                }
                return new CalcLiteralExpression(new RawValue(token.ToValue()));
            }

            return null;
        }

        private void SkipWhitespace()
        {
            while (_position < _tokens.Count && _tokens[_position].Type == TokenType.Whitespace)
                _position++;
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
    }
}
