using System;
using System.Collections.Generic;

namespace ExCSS
{
    internal sealed class TypedFunctionValueConverter<T> : IValueConverter
        where T : struct, IFormattable
    {
        private readonly IValueConverter _arguments;
        private readonly string _name;
        private readonly Func<IPropertyValue[], T?> _extractor;

        public TypedFunctionValueConverter(string name, IValueConverter arguments, Func<IPropertyValue[], T?> extractor)
        {
            _name = name;
            _arguments = arguments;
            _extractor = extractor;
        }

        public IPropertyValue Convert(IEnumerable<Token> value)
        {
            var function = value.OnlyOrDefault() as FunctionToken;

            if (!Check(function)) return null;

            var args = _arguments.Convert(function.ArgumentTokens);
            if (args == null) return null;

            var argValues = ExtractArguments(args);
            if (argValues == null) return null;

            var typedValue = _extractor(argValues);
            if (!typedValue.HasValue) return null;

            return new TypedFunctionValue(_name, typedValue.Value, args, value);
        }

        public IPropertyValue Construct(Property[] properties)
        {
            return properties.Guard<TypedFunctionValue>();
        }

        private bool Check(FunctionToken function)
        {
            return function != null && function.Data.Equals(_name, StringComparison.OrdinalIgnoreCase);
        }

        private IPropertyValue[] ExtractArguments(IPropertyValue args)
        {
            if (args is IArgumentsPropertyValue argsValue)
            {
                return argsValue.Arguments;
            }
            return new[] { args };
        }

        private sealed class TypedFunctionValue : IPropertyValue, ITypedPropertyValue
        {
            private readonly string _name;
            private readonly T _value;
            private readonly IPropertyValue _arguments;

            public TypedFunctionValue(string name, T value, IPropertyValue arguments, IEnumerable<Token> tokens)
            {
                _name = name;
                _value = value;
                _arguments = arguments;
                Original = new TokenValue(tokens);
            }

            public string CssText => _name.StylesheetFunction(_arguments.CssText);

            public TokenValue Original { get; }

            public TokenValue ExtractFor(string name)
            {
                return Original;
            }

            public object GetValue() => _value;
        }
    }

    internal interface IArgumentsPropertyValue
    {
        IPropertyValue[] Arguments { get; }
    }
}
