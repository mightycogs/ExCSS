namespace ExCSS
{
    internal sealed class BackdropFilterProperty : Property
    {
        private static readonly IValueConverter TheConverter = Converters.FilterConverter.Or(Converters.Any);

        internal BackdropFilterProperty()
            : base(PropertyNames.BackdropFilter)
        {
        }

        internal override IValueConverter Converter => TheConverter;
    }
}
