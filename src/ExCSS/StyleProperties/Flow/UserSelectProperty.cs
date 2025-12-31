namespace ExCSS
{
    internal sealed class UserSelectProperty : Property
    {
        internal UserSelectProperty()
            : base(PropertyNames.UserSelect)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}
