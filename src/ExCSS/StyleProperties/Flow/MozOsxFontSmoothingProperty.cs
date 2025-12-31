namespace ExCSS
{
    internal sealed class MozOsxFontSmoothingProperty : Property
    {
        internal MozOsxFontSmoothingProperty()
            : base(PropertyNames.MozOsxFontSmoothing)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}
