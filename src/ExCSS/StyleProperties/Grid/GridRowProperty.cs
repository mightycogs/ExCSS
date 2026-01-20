namespace ExCSS
{
    internal sealed class GridRowProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = Converters.Any;

        internal GridRowProperty()
            : base(PropertyNames.GridRow)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
