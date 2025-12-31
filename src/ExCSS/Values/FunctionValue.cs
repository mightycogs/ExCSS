using System;

namespace ExCSS
{
    public readonly struct FunctionValue : IStyleValue, IEquatable<FunctionValue>
    {
        public string Name { get; }
        public string Arguments { get; }

        public FunctionValue(string name, string arguments)
        {
            Name = name ?? string.Empty;
            Arguments = arguments ?? string.Empty;
        }

        public string CssText => $"{Name}({Arguments})";
        public StyleValueType Type => StyleValueType.Function;

        public bool Equals(FunctionValue other) =>
            string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(Arguments, other.Arguments, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is FunctionValue other && Equals(other);
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + StringComparer.OrdinalIgnoreCase.GetHashCode(Name ?? string.Empty);
                hash = hash * 31 + (Arguments?.GetHashCode() ?? 0);
                return hash;
            }
        }
        public override string ToString() => CssText;

        public static bool operator ==(FunctionValue left, FunctionValue right) => left.Equals(right);
        public static bool operator !=(FunctionValue left, FunctionValue right) => !left.Equals(right);
    }
}
