namespace ExCSS
{
    /// <summary>
    /// WARNING: Uses Converters.Any intentionally!
    /// See GridColumnStartProperty for explanation.
    /// </summary>
    internal sealed class GridRowStartProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.Any;

        internal GridRowStartProperty()
            : base(PropertyNames.GridRowStart)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
