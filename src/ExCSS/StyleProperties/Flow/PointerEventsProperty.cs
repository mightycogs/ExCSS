namespace ExCSS
{
    internal sealed class PointerEventsProperty : Property
    {
        internal PointerEventsProperty()
            : base(PropertyNames.PointerEvents)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}
