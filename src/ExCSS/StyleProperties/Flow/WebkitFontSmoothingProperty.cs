namespace ExCSS
{
    internal sealed class WebkitFontSmoothingProperty : Property
    {
        internal WebkitFontSmoothingProperty()
            : base(PropertyNames.WebkitFontSmoothing)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}
