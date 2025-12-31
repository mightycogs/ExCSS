namespace ExCSS
{
    internal sealed class MozAppearanceProperty : Property
    {
        internal MozAppearanceProperty()
            : base(PropertyNames.MozAppearance)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}
