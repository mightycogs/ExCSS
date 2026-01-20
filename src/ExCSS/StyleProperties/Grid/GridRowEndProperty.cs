namespace ExCSS
{
    /// <summary>
    /// WARNING: Uses Converters.Any intentionally!
    /// See GridColumnStartProperty for explanation.
    /// </summary>
    internal sealed class GridRowEndProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.Any;

        internal GridRowEndProperty()
            : base(PropertyNames.GridRowEnd)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
