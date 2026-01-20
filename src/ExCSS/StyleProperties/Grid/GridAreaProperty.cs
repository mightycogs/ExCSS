namespace ExCSS
{
    internal sealed class GridAreaProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = Converters.Any
            .OrAuto()
            .OrGlobalValue();

        internal GridAreaProperty()
            : base(PropertyNames.GridArea)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
