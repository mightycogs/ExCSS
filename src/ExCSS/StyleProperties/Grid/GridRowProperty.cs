namespace ExCSS
{
    /// <summary>
    /// Shorthand for grid-row-start / grid-row-end.
    /// Uses GridLineValueConverter which implements ExtractFor() to split "start / end" syntax.
    /// See GridLineValueConverter for architecture documentation.
    /// </summary>
    internal sealed class GridRowProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = new GridLineValueConverter(
            PropertyNames.GridRowStart,
            PropertyNames.GridRowEnd);

        internal GridRowProperty()
            : base(PropertyNames.GridRow)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
