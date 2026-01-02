namespace ExCSS
{
    internal sealed class AspectRatioProperty : Property
    {
        private static readonly IValueConverter TheConverter = Converters.AspectRatioValueConverter.Or(Keywords.Auto);

        internal AspectRatioProperty()
            : base(PropertyNames.AspectRatio)
        {
        }

        internal override IValueConverter Converter => TheConverter;
    }
}
