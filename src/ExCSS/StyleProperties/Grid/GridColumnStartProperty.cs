namespace ExCSS
{
    internal sealed class GridColumnStartProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.IntegerConverter
            .Or(Converters.IdentifierConverter)
            .OrAuto()
            .OrGlobalValue();

        internal GridColumnStartProperty()
            : base(PropertyNames.GridColumnStart)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
