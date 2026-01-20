namespace ExCSS
{
    internal sealed class GridRowStartProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.IntegerConverter
            .Or(Converters.IdentifierConverter)
            .OrAuto()
            .OrGlobalValue();

        internal GridRowStartProperty()
            : base(PropertyNames.GridRowStart)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
