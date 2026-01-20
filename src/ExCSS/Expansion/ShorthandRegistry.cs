using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    /// <summary>
    /// Registry for shorthand property expanders.
    /// </summary>
    public static class ShorthandRegistry
    {
        private static readonly ConcurrentDictionary<string, IShorthandExpander> _expanders =
            new(StringComparer.OrdinalIgnoreCase);

        static ShorthandRegistry()
        {
            Register(new BoxModelExpander("margin"));
            Register(new BoxModelExpander("padding"));
            Register(new InsetExpander());
            Register(new BorderExpander());
            Register(new BackgroundExpander());
            Register(new BorderRadiusExpander());
            Register(new FlexExpander());
            Register(new FlexFlowExpander());
            Register(new GapExpander());
            Register(new ListStyleExpander());
            Register(new OutlineExpander());
            Register(new PlaceExpander(PropertyNames.PlaceContent, PropertyNames.AlignContent, PropertyNames.JustifyContent));
            Register(new PlaceExpander(PropertyNames.PlaceItems, PropertyNames.AlignItems, PropertyNames.JustifyItems));
            Register(new PlaceExpander(PropertyNames.PlaceSelf, PropertyNames.AlignSelf, PropertyNames.JustifySelf));
            Register(new TextDecorationExpander());
            Register(new BorderSideExpander(PropertyNames.BorderTop, PropertyNames.BorderTopWidth, PropertyNames.BorderTopStyle, PropertyNames.BorderTopColor));
            Register(new BorderSideExpander(PropertyNames.BorderRight, PropertyNames.BorderRightWidth, PropertyNames.BorderRightStyle, PropertyNames.BorderRightColor));
            Register(new BorderSideExpander(PropertyNames.BorderBottom, PropertyNames.BorderBottomWidth, PropertyNames.BorderBottomStyle, PropertyNames.BorderBottomColor));
            Register(new BorderSideExpander(PropertyNames.BorderLeft, PropertyNames.BorderLeftWidth, PropertyNames.BorderLeftStyle, PropertyNames.BorderLeftColor));
            Register(new BorderLogicalExpander(PropertyNames.BorderInline,
                PropertyNames.BorderInlineStartWidth, PropertyNames.BorderInlineStartStyle, PropertyNames.BorderInlineStartColor,
                PropertyNames.BorderInlineEndWidth, PropertyNames.BorderInlineEndStyle, PropertyNames.BorderInlineEndColor));
            Register(new BorderLogicalExpander(PropertyNames.BorderBlock,
                PropertyNames.BorderBlockStartWidth, PropertyNames.BorderBlockStartStyle, PropertyNames.BorderBlockStartColor,
                PropertyNames.BorderBlockEndWidth, PropertyNames.BorderBlockEndStyle, PropertyNames.BorderBlockEndColor));
            Register(new LogicalPairExpander(PropertyNames.MarginInline, PropertyNames.MarginInlineStart, PropertyNames.MarginInlineEnd));
            Register(new LogicalPairExpander(PropertyNames.MarginBlock, PropertyNames.MarginBlockStart, PropertyNames.MarginBlockEnd));
            Register(new LogicalPairExpander(PropertyNames.PaddingInline, PropertyNames.PaddingInlineStart, PropertyNames.PaddingInlineEnd));
            Register(new LogicalPairExpander(PropertyNames.PaddingBlock, PropertyNames.PaddingBlockStart, PropertyNames.PaddingBlockEnd));
            Register(new ColumnRuleExpander());
            Register(new ColumnsExpander());
            Register(new AnimationExpander());
            Register(new TransitionExpander());
            Register(new FontExpander());
            Register(new GridLineExpander(PropertyNames.GridColumn, PropertyNames.GridColumnStart, PropertyNames.GridColumnEnd));
            Register(new GridLineExpander(PropertyNames.GridRow, PropertyNames.GridRowStart, PropertyNames.GridRowEnd));
        }

        /// <summary>
        /// Register a custom shorthand expander.
        /// </summary>
        public static void Register(IShorthandExpander expander)
        {
            if (expander == null) return;
            foreach (var name in expander.ShorthandNames)
            {
                _expanders.AddOrUpdate(name, expander, (_, __) => expander);
            }
        }

        /// <summary>
        /// Check if a property is a shorthand.
        /// </summary>
        public static bool IsShorthand(string propertyName)
        {
            return !string.IsNullOrEmpty(propertyName) && _expanders.ContainsKey(propertyName);
        }

        /// <summary>
        /// Get the expander for a shorthand property.
        /// </summary>
        public static IShorthandExpander GetExpander(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return null;
            return _expanders.TryGetValue(propertyName, out var expander) ? expander : null;
        }

        /// <summary>
        /// Expand a shorthand property to its longhand values.
        /// Returns single-entry dictionary with original if not a shorthand.
        /// </summary>
        public static IReadOnlyDictionary<string, IStyleValue> Expand(string propertyName, IStyleValue value)
        {
            if (string.IsNullOrEmpty(propertyName) || value == null)
                return new Dictionary<string, IStyleValue>();

            if (_expanders.TryGetValue(propertyName, out var expander))
                return expander.Expand(value);

            return new Dictionary<string, IStyleValue> { [propertyName] = value };
        }

        /// <summary>
        /// Get longhand property names for a shorthand.
        /// </summary>
        public static IEnumerable<string> GetLonghands(string shorthandName)
        {
            if (string.IsNullOrEmpty(shorthandName)) return Enumerable.Empty<string>();
            return _expanders.TryGetValue(shorthandName, out var expander)
                ? expander.LonghandNames
                : Enumerable.Empty<string>();
        }

        /// <summary>
        /// Get all registered shorthand names.
        /// </summary>
        public static IEnumerable<string> GetAllShorthands() => _expanders.Keys;
    }
}
