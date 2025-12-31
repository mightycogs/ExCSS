using System.Collections.Generic;

namespace ExCSS
{
    /// <summary>
    /// Expands CSS shorthand properties to longhand properties.
    /// </summary>
    public interface IShorthandExpander
    {
        /// <summary>
        /// Property names this expander handles.
        /// </summary>
        IReadOnlyList<string> ShorthandNames { get; }

        /// <summary>
        /// Longhand property names this shorthand expands to.
        /// </summary>
        IReadOnlyList<string> LonghandNames { get; }

        /// <summary>
        /// Expand shorthand value to longhand values.
        /// </summary>
        IReadOnlyDictionary<string, IStyleValue> Expand(IStyleValue value);
    }
}
