namespace ExCSS
{
    public sealed class GradientFunctionValue : IStyleValue, IFunctionValue
    {
        public string Name { get; }
        public string Arguments { get; }
        public bool IsRepeating { get; }

        public GradientFunctionValue(string name, string arguments)
        {
            Name = name ?? string.Empty;
            Arguments = arguments ?? string.Empty;
            IsRepeating = Name.StartsWith("repeating-");
        }

        public string CssText => $"{Name}({Arguments})";
        public StyleValueType Type => StyleValueType.Gradient;

        public override string ToString() => CssText;
    }
}
