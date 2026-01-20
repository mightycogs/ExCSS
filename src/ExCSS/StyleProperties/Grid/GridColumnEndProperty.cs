namespace ExCSS
{
    internal sealed class GridColumnEndProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.IntegerConverter
            .Or(Converters.IdentifierConverter)
            .OrAuto()
            .OrGlobalValue();

        internal GridColumnEndProperty()
            : base(PropertyNames.GridColumnEnd)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
