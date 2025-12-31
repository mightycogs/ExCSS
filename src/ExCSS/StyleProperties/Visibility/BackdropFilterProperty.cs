namespace ExCSS
{
    internal sealed class BackdropFilterProperty : Property
    {
        internal BackdropFilterProperty()
            : base(PropertyNames.BackdropFilter)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}
