namespace ExCSS
{
    public sealed class CalcValue : IStyleValue, IFunctionValue
    {
        public string Name => "calc";
        public ICalcExpression Root { get; }
        public string Expression => Root?.CssText ?? string.Empty;

        public CalcValue(ICalcExpression root)
        {
            Root = root;
        }

        public CalcValue(string expression)
        {
            Root = new CalcRawExpression(expression ?? string.Empty);
        }

        public string CssText => $"calc({Expression})";
        public StyleValueType Type => StyleValueType.Function;

        public override string ToString() => CssText;

        public static CalcValue FromString(string expression)
        {
            return new CalcValue(expression);
        }
    }

    public sealed class CalcRawExpression : ICalcExpression
    {
        private readonly string _expression;

        public CalcRawExpression(string expression)
        {
            _expression = expression;
        }

        public string CssText => _expression;
    }
}
