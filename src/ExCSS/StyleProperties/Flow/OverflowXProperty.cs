namespace ExCSS
{
    internal sealed class OverflowXProperty : Property
    {
        internal OverflowXProperty()
            : base(PropertyNames.OverflowX)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}
