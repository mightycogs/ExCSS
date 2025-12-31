namespace ExCSS
{
    internal sealed class WebkitAppearanceProperty : Property
    {
        internal WebkitAppearanceProperty()
            : base(PropertyNames.WebkitAppearance)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}
