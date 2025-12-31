namespace ExCSS
{
    /// <summary>
    /// Extension methods for accessing typed CSS values from style declarations.
    /// </summary>
    public static class StyleDeclarationExtensions
    {
        /// <summary>
        /// Get a property's typed value.
        /// </summary>
        public static IStyleValue GetTypedValue(this IProperties style, string propertyName)
        {
            if (style is StyleDeclaration declaration)
            {
                var prop = declaration.GetProperty(propertyName);
                return prop?.TypedValue;
            }
            return null;
        }

        /// <summary>
        /// Try to get a property value as Length.
        /// </summary>
        public static Length? GetLength(this IProperties style, string propertyName)
        {
            var value = style.GetTypedValue(propertyName);
            return value is Length len ? len : (Length?)null;
        }

        /// <summary>
        /// Try to get a property value as Color.
        /// </summary>
        public static Color? GetColor(this IProperties style, string propertyName)
        {
            var value = style.GetTypedValue(propertyName);
            return value is Color col ? col : (Color?)null;
        }

        /// <summary>
        /// Try to get a property value as Angle.
        /// </summary>
        public static Angle? GetAngle(this IProperties style, string propertyName)
        {
            var value = style.GetTypedValue(propertyName);
            return value is Angle a ? a : (Angle?)null;
        }

        /// <summary>
        /// Try to get a property value as Time.
        /// </summary>
        public static Time? GetTime(this IProperties style, string propertyName)
        {
            var value = style.GetTypedValue(propertyName);
            return value is Time t ? t : (Time?)null;
        }

        /// <summary>
        /// Try to get a property value as Percent.
        /// </summary>
        public static Percent? GetPercent(this IProperties style, string propertyName)
        {
            var value = style.GetTypedValue(propertyName);
            return value is Percent p ? p : (Percent?)null;
        }

        /// <summary>
        /// Check if property contains a var() reference.
        /// </summary>
        public static bool HasVariableReference(this IProperties style, string propertyName)
        {
            var value = style.GetTypedValue(propertyName);
            return value is VarValue;
        }

        // Convenience accessors for common properties
        public static Length? GetMarginTop(this IProperties style) => style.GetLength("margin-top");
        public static Length? GetMarginRight(this IProperties style) => style.GetLength("margin-right");
        public static Length? GetMarginBottom(this IProperties style) => style.GetLength("margin-bottom");
        public static Length? GetMarginLeft(this IProperties style) => style.GetLength("margin-left");

        public static Length? GetPaddingTop(this IProperties style) => style.GetLength("padding-top");
        public static Length? GetPaddingRight(this IProperties style) => style.GetLength("padding-right");
        public static Length? GetPaddingBottom(this IProperties style) => style.GetLength("padding-bottom");
        public static Length? GetPaddingLeft(this IProperties style) => style.GetLength("padding-left");

        public static Color? GetBackgroundColor(this IProperties style) => style.GetColor("background-color");
        public static Color? GetBorderColor(this IProperties style) => style.GetColor("border-color");
    }
}
