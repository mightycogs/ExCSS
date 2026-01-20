namespace ExCSS
{
    /// <summary>
    /// Shorthand for grid-column-start / grid-column-end.
    /// Uses GridLineValueConverter which implements ExtractFor() to split "start / end" syntax.
    /// See GridLineValueConverter for architecture documentation.
    /// </summary>
    internal sealed class GridColumnProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = new GridLineValueConverter(
            PropertyNames.GridColumnStart,
            PropertyNames.GridColumnEnd);

        internal GridColumnProperty()
            : base(PropertyNames.GridColumn)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
