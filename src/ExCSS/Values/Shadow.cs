using System.Collections.Generic;

namespace ExCSS
{
    public sealed class Shadow : IStyleValue, ICompositeValue
    {
        public Shadow(bool inset, Length offsetX, Length offsetY, Length blurRadius, Length spreadRadius, Color color)
        {
            IsInset = inset;
            OffsetX = offsetX;
            OffsetY = offsetY;
            BlurRadius = blurRadius;
            SpreadRadius = spreadRadius;
            Color = color;
        }

        /// <summary>
        /// Try to parse a Shadow from a StyleValueList or IEnumerable of style values.
        /// Returns null if the values cannot be parsed as a valid shadow.
        /// </summary>
        /// <param name="values">Collection of style values (typically from TypedValue)</param>
        /// <returns>Shadow instance or null if parsing fails</returns>
        public static Shadow TryParse(IEnumerable<IStyleValue> values)
        {
            if (values == null) return null;

            var lengths = new List<Length>();
            Color? color = null;
            bool isInset = false;

            foreach (var item in values)
            {
                switch (item)
                {
                    case Length len:
                        lengths.Add(len);
                        break;
                    case Number num when lengths.Count < 4:
                        lengths.Add(new Length((float)num.Value, Length.Unit.Px));
                        break;
                    case Color col:
                        color = col;
                        break;
                    case KeywordValue kw when kw.CssText?.ToLowerInvariant() == "inset":
                        isInset = true;
                        break;
                    case RawValue raw when raw.CssText?.ToLowerInvariant() == "inset":
                        isInset = true;
                        break;
                    case StyleValueList innerList:
                        var innerResult = TryParseInner(innerList, ref lengths, ref color, ref isInset);
                        if (!innerResult) continue;
                        break;
                }
            }

            if (lengths.Count < 2) return null;

            return new Shadow(
                isInset,
                lengths[0],
                lengths[1],
                lengths.Count > 2 ? lengths[2] : Length.Zero,
                lengths.Count > 3 ? lengths[3] : Length.Zero,
                color ?? Color.Black
            );
        }

        private static bool TryParseInner(StyleValueList list, ref List<Length> lengths, ref Color? color, ref bool isInset)
        {
            foreach (var inner in list)
            {
                switch (inner)
                {
                    case Length innerLen:
                        lengths.Add(innerLen);
                        break;
                    case Number innerNum when lengths.Count < 4:
                        lengths.Add(new Length((float)innerNum.Value, Length.Unit.Px));
                        break;
                    case Color innerCol:
                        color = innerCol;
                        break;
                    case KeywordValue kw when kw.CssText?.ToLowerInvariant() == "inset":
                        isInset = true;
                        break;
                    case RawValue raw when raw.CssText?.ToLowerInvariant() == "inset":
                        isInset = true;
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// Try to parse a Shadow from a single IStyleValue (handles Shadow, StyleValueList, etc.)
        /// Also accepts Property to check for 'inset' in original CSS text.
        /// </summary>
        public static Shadow TryParse(object value, string cssText = null)
        {
            // Check for 'inset' in CSS text since it may not be in the TypedValue
            bool hasInsetInCss = cssText?.ToLowerInvariant().Contains("inset") == true;

            Shadow result = value switch
            {
                Shadow s => s,
                StyleValueList list => TryParse((IEnumerable<IStyleValue>)list),
                IEnumerable<IStyleValue> enumerable => TryParse(enumerable),
                _ => null
            };

            // Apply inset from CSS text if not detected in values
            if (result != null && hasInsetInCss && !result.IsInset)
            {
                result = new Shadow(true, result.OffsetX, result.OffsetY,
                    result.BlurRadius, result.SpreadRadius, result.Color);
            }

            return result;
        }

        public Color Color { get; }
        public Length OffsetX { get; }
        public Length OffsetY { get; }
        public Length BlurRadius { get; }
        public Length SpreadRadius { get; }
        public bool IsInset { get; }

        public string CssText => ToString();

        public StyleValueType Type => StyleValueType.Shadow;

        public override string ToString()
        {
            var parts = new System.Collections.Generic.List<string>();
            if (IsInset) parts.Add("inset");
            parts.Add(OffsetX.ToString());
            parts.Add(OffsetY.ToString());
            if (BlurRadius.Value != 0) parts.Add(BlurRadius.ToString());
            if (SpreadRadius.Value != 0) parts.Add(SpreadRadius.ToString());
            parts.Add(Color.ToString());
            return string.Join(" ", parts);
        }
    }
}