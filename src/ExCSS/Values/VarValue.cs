using System.Collections.Generic;

namespace ExCSS
{
    /// <summary>
    /// CSS var() function value with optional fallback.
    /// </summary>
    public sealed class VarValue : IStyleValue, IFunctionValue
    {
        public string Name => "var";
        public string VariableName { get; }
        public IStyleValue Fallback { get; }

        public VarValue(string variableName, IStyleValue fallback = null)
        {
            if (string.IsNullOrEmpty(variableName))
            {
                VariableName = "--";
            }
            else
            {
                VariableName = variableName.StartsWith("--") ? variableName : "--" + variableName;
            }
            Fallback = fallback;
        }

        public string CssText => Fallback != null
            ? $"var({VariableName}, {Fallback.CssText})"
            : $"var({VariableName})";

        public StyleValueType Type => StyleValueType.Function;

        public override string ToString() => CssText;

        /// <summary>
        /// Resolve this var() reference using provided variables dictionary.
        /// Returns fallback if variable not found, or this if no fallback.
        /// </summary>
        public IStyleValue Resolve(IReadOnlyDictionary<string, IStyleValue> variables)
        {
            if (variables != null && variables.TryGetValue(VariableName, out var resolved))
                return resolved;
            return Fallback ?? this;
        }
    }
}
