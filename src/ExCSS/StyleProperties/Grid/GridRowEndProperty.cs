namespace ExCSS
{
    internal sealed class GridRowEndProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.IntegerConverter
            .Or(Converters.IdentifierConverter)
            .OrAuto()
            .OrGlobalValue();

        internal GridRowEndProperty()
            : base(PropertyNames.GridRowEnd)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
