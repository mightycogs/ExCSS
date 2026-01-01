using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    public sealed class FlexExpander : IShorthandExpander
    {
        private const string FlexGrow = "flex-grow";
        private const string FlexShrink = "flex-shrink";
        private const string FlexBasis = "flex-basis";

        public IReadOnlyList<string> ShorthandNames => new[] { "flex" };
        public IReadOnlyList<string> LonghandNames => new[] { FlexGrow, FlexShrink, FlexBasis };

        public IReadOnlyDictionary<string, IStyleValue> Expand(IStyleValue value)
        {
            var result = new Dictionary<string, IStyleValue>(StringComparer.OrdinalIgnoreCase);

            if (value == null)
                return result;

            if (value is KeywordValue kw)
            {
                var keyword = kw.Value;
                if (string.Equals(keyword, "none", StringComparison.OrdinalIgnoreCase))
                {
                    result[FlexGrow] = Number.Zero;
                    result[FlexShrink] = Number.Zero;
                    result[FlexBasis] = KeywordValue.Auto;
                    return result;
                }
                if (string.Equals(keyword, "auto", StringComparison.OrdinalIgnoreCase))
                {
                    result[FlexGrow] = Number.One;
                    result[FlexShrink] = Number.One;
                    result[FlexBasis] = KeywordValue.Auto;
                    return result;
                }
                if (string.Equals(keyword, "initial", StringComparison.OrdinalIgnoreCase))
                {
                    result[FlexGrow] = Number.Zero;
                    result[FlexShrink] = Number.One;
                    result[FlexBasis] = KeywordValue.Auto;
                    return result;
                }
                if (string.Equals(keyword, "inherit", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(keyword, "unset", StringComparison.OrdinalIgnoreCase))
                {
                    result[FlexGrow] = kw;
                    result[FlexShrink] = kw;
                    result[FlexBasis] = kw;
                    return result;
                }
            }

            var values = ExpanderHelpers.ExtractValues(value);
            if (values.Length == 0)
                return result;

            IStyleValue grow = Number.Zero;
            IStyleValue shrink = Number.One;
            IStyleValue basis = Number.Zero;

            int numberCount = 0;
            foreach (var v in values)
            {
                if (IsNumber(v))
                {
                    var num = (Number)v;
                    if (num.Value < 0)
                        return result;

                    numberCount++;
                    if (numberCount == 1)
                        grow = v;
                    else if (numberCount == 2)
                        shrink = v;
                }
                else if (IsBasisValue(v))
                {
                    basis = v;
                    if (numberCount == 0)
                    {
                        grow = Number.One;
                    }
                }
            }

            if (numberCount == 1 && !HasBasisValue(values))
            {
                basis = Number.Zero;
            }

            result[FlexGrow] = grow;
            result[FlexShrink] = shrink;
            result[FlexBasis] = basis;
            return result;
        }

        private static bool IsNumber(IStyleValue value)
        {
            return value is Number;
        }

        private static bool IsBasisValue(IStyleValue value)
        {
            if (value is Length || value is Percent || value is CalcValue)
                return true;

            if (value is KeywordValue kw)
            {
                var keyword = kw.Value;
                return string.Equals(keyword, "auto", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(keyword, "content", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(keyword, "min-content", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(keyword, "max-content", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(keyword, "fit-content", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static bool HasBasisValue(IStyleValue[] values)
        {
            return values.Any(IsBasisValue);
        }
    }
}
