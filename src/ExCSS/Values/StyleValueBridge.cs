namespace ExCSS
{
    /// <summary>
    /// Bridge between internal IPropertyValue and public IStyleValue.
    /// </summary>
    internal static class StyleValueBridge
    {
        /// <summary>
        /// Convert internal IPropertyValue to public IStyleValue.
        /// </summary>
        public static IStyleValue ToStyleValue(IPropertyValue propertyValue)
        {
            if (propertyValue == null)
                return null;

            // Try to extract typed value from StructValue wrapper
            if (propertyValue is ITypedPropertyValue typed)
            {
                var value = typed.GetValue();
                if (value is IStyleValue styleValue)
                    return styleValue;
            }

            // Fallback to raw value
            return new RawValue(propertyValue.CssText);
        }
    }
}
