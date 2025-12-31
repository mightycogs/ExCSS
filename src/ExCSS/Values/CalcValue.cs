namespace ExCSS
{
    /// <summary>
    /// CSS calc() function value.
    /// </summary>
    public sealed class CalcValue : IStyleValue, IFunctionValue
    {
        public string Name => "calc";
        public string Expression { get; }

        public CalcValue(string expression)
        {
            Expression = expression ?? string.Empty;
        }

        public string CssText => $"calc({Expression})";
        public StyleValueType Type => StyleValueType.Function;

        public override string ToString() => CssText;
    }
}
