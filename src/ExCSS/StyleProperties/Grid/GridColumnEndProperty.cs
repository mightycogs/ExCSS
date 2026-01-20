namespace ExCSS
{
    /// <summary>
    /// WARNING: Uses Converters.Any intentionally!
    /// See GridColumnStartProperty for explanation.
    /// </summary>
    internal sealed class GridColumnEndProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.Any;

        internal GridColumnEndProperty()
            : base(PropertyNames.GridColumnEnd)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
