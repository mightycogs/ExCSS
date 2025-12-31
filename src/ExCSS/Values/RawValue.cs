namespace ExCSS
{
    /// <summary>
    /// Raw/unparsed CSS value. Used as fallback when typed parsing fails.
    /// </summary>
    public sealed class RawValue : IStyleValue
    {
        public string Value { get; }

        public RawValue(string value)
        {
            Value = value ?? string.Empty;
        }

        public string CssText => Value;
        public StyleValueType Type => StyleValueType.Unknown;

        public override string ToString() => Value;
    }
}
