namespace ExCSS
{
    internal sealed class CustomProperty : Property
    {
        internal CustomProperty(string name)
            : base(name, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}
