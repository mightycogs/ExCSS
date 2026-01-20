namespace ExCSS
{
    internal sealed class GridTemplateColumnsProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.Any
            .OrNone()
            .OrGlobalValue();

        internal GridTemplateColumnsProperty()
            : base(PropertyNames.GridTemplateColumns)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
