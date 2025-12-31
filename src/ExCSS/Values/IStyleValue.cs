namespace ExCSS
{
    /// <summary>
    /// Represents a strongly-typed CSS value.
    /// </summary>
    public interface IStyleValue
    {
        /// <summary>
        /// CSS text representation of the value.
        /// </summary>
        string CssText { get; }

        /// <summary>
        /// The type of this value for pattern matching.
        /// </summary>
        StyleValueType Type { get; }
    }

    public enum StyleValueType : byte
    {
        Length,
        Color,
        Number,
        Percent,
        Angle,
        Time,
        Frequency,
        Resolution,
        Keyword,
        String,
        Function,
        List,
        Tuple,
        Shadow,
        Gradient,
        Transform,
        Unknown
    }

    /// <summary>Marker for primitive values (Length, Color, Number)</summary>
    public interface IPrimitiveValue : IStyleValue { }

    /// <summary>Marker for composite values (border, background)</summary>
    public interface ICompositeValue : IStyleValue { }

    /// <summary>Marker for function values (var, calc, url)</summary>
    public interface IFunctionValue : IStyleValue
    {
        string Name { get; }
    }
}
