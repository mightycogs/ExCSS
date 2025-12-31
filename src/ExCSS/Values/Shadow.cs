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