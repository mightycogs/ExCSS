namespace ExCSS
{
    internal sealed class AppearanceProperty : Property
    {
        internal AppearanceProperty()
            : base(PropertyNames.Appearance)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}
