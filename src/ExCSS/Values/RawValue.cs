namespace ExCSS
{
    /// <summary>
    /// Raw/unparsed CSS value. Used for:
    /// 1. Legitimate deferred evaluation (var(), calc(), env()) - IsParseFailure = false
    /// 2. Parse failures where typed parsing failed - IsParseFailure = true
    /// </summary>
    public sealed class RawValue : IStyleValue
    {
        public string Value { get; }

        /// <summary>
        /// True if this RawValue represents a parse failure (typed parsing failed).
        /// False if this is legitimate deferred content (var(), calc(), custom properties).
        /// </summary>
        public bool IsParseFailure { get; }

        public RawValue(string value, bool isParseFailure = false)
        {
            Value = value ?? string.Empty;
            IsParseFailure = isParseFailure;
        }

        public string CssText => Value;
        public StyleValueType Type => StyleValueType.Unknown;

        public override string ToString() => IsParseFailure
            ? $"RawValue(\"{Value}\", IsParseFailure=true)"
            : Value;
    }
}
