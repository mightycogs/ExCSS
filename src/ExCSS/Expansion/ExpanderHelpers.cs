using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    internal static class ExpanderHelpers
    {
        public static IStyleValue[] ExtractValues(IStyleValue value)
        {
            return value switch
            {
                StyleValueTuple tuple => tuple.ToArray(),
                IReadOnlyList<IStyleValue> list => list.ToArray(),
                null => Array.Empty<IStyleValue>(),
                _ => new[] { value }
            };
        }
    }
}
