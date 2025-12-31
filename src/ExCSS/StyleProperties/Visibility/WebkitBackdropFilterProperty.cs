namespace ExCSS
{
    internal sealed class WebkitBackdropFilterProperty : Property
    {
        internal WebkitBackdropFilterProperty()
            : base(PropertyNames.WebkitBackdropFilter)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}
