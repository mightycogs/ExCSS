namespace ExCSS
{
    internal sealed class OverflowYProperty : Property
    {
        internal OverflowYProperty()
            : base(PropertyNames.OverflowY)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}
