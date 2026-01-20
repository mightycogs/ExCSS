namespace ExCSS
{
    internal sealed class GridTemplateRowsProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.Any
            .OrNone()
            .OrGlobalValue();

        internal GridTemplateRowsProperty()
            : base(PropertyNames.GridTemplateRows)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
