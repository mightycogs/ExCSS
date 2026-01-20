namespace ExCSS
{
    /// <summary>
    /// WARNING: Uses Converters.Any intentionally!
    /// Grid line values can be: auto | integer | "span N" | custom-ident | "span custom-ident"
    /// A strict converter like IntegerConverter.Or(Identifier) rejects "span 2" because
    /// it's a multi-token sequence. Converters.Any accepts any value; actual parsing
    /// happens in the consumer (e.g., MightyUI's PropertyApplicator).
    /// </summary>
    internal sealed class GridColumnStartProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.Any;

        internal GridColumnStartProperty()
            : base(PropertyNames.GridColumnStart)
        { }

        internal override IValueConverter Converter => StyleConverter;
    }
}
