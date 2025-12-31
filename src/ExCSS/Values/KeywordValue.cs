using System;

namespace ExCSS
{
    /// <summary>
    /// CSS keyword value (auto, none, inherit, initial, solid, etc.)
    /// </summary>
    public readonly struct KeywordValue : IStyleValue, IPrimitiveValue, IEquatable<KeywordValue>
    {
        public static readonly KeywordValue Auto = new("auto");
        public static readonly KeywordValue None = new("none");
        public static readonly KeywordValue Inherit = new("inherit");
        public static readonly KeywordValue Initial = new("initial");
        public static readonly KeywordValue Unset = new("unset");
        public static readonly KeywordValue CurrentColor = new("currentColor");

        public string Value { get; }

        public KeywordValue(string value) => Value = value ?? string.Empty;

        public string CssText => Value;
        public StyleValueType Type => StyleValueType.Keyword;

        public bool Equals(KeywordValue other) =>
            string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj) => obj is KeywordValue other && Equals(other);
        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value ?? string.Empty);
        public override string ToString() => Value;

        public static bool operator ==(KeywordValue left, KeywordValue right) => left.Equals(right);
        public static bool operator !=(KeywordValue left, KeywordValue right) => !left.Equals(right);

        // Convenience checks
        public bool IsAuto => string.Equals(Value, "auto", StringComparison.OrdinalIgnoreCase);
        public bool IsNone => string.Equals(Value, "none", StringComparison.OrdinalIgnoreCase);
        public bool IsInherit => string.Equals(Value, "inherit", StringComparison.OrdinalIgnoreCase);
        public bool IsInitial => string.Equals(Value, "initial", StringComparison.OrdinalIgnoreCase);
    }
}
