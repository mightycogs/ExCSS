namespace ExCSS
{
    internal sealed class GridColumnProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = Converters.Any;

        internal GridColumnProperty()
            : base(PropertyNames.GridColumn)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
