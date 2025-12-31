namespace ExCSS
{
    internal sealed class WebkitLineClampProperty : Property
    {
        internal WebkitLineClampProperty()
            : base(PropertyNames.WebkitLineClamp)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}
