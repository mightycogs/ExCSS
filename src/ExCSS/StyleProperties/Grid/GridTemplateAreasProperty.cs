namespace ExCSS
{
    internal sealed class GridTemplateAreasProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.Any
            .OrNone()
            .OrGlobalValue();

        internal GridTemplateAreasProperty()
            : base(PropertyNames.GridTemplateAreas)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
