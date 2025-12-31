namespace ExCSS
{
    internal sealed class WebkitBoxOrientProperty : Property
    {
        internal WebkitBoxOrientProperty()
            : base(PropertyNames.WebkitBoxOrient)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}
