namespace ExCSS
{
    public struct GradientStop : IStyleValue
    {
        public GradientStop(Color color, Length location)
        {
            Color = color;
            Location = location;
        }

        public Color Color { get; }
        public Length Location { get; }

        public string CssText
        {
            get
            {
                if (Location == Length.Zero || Location == Length.Missing)
                    return Color.CssText;
                return $"{Color.CssText} {Location.CssText}";
            }
        }

        public StyleValueType Type => StyleValueType.Gradient;

        public override string ToString() => CssText;
    }
}
