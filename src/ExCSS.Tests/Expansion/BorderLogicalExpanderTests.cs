using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class BorderLogicalExpanderTests
    {
        [Fact]
        public void BorderInline_SetsStartAndEnd()
        {
            var expander = new BorderLogicalExpander(
                PropertyNames.BorderInline,
                PropertyNames.BorderInlineStartWidth, PropertyNames.BorderInlineStartStyle, PropertyNames.BorderInlineStartColor,
                PropertyNames.BorderInlineEndWidth, PropertyNames.BorderInlineEndStyle, PropertyNames.BorderInlineEndColor);

            var result = expander.Expand(new StyleValueTuple(
                new Length(2, Length.Unit.Px),
                new KeywordValue("dashed"),
                Color.Red
            ));

            Assert.Equal("2px", result[PropertyNames.BorderInlineStartWidth].CssText);
            Assert.Equal("dashed", result[PropertyNames.BorderInlineEndStyle].CssText);
            Assert.Equal(Color.Red, result[PropertyNames.BorderInlineEndColor]);
        }

        [Fact]
        public void BorderBlock_GlobalKeyword_Propagates()
        {
            var expander = new BorderLogicalExpander(
                PropertyNames.BorderBlock,
                PropertyNames.BorderBlockStartWidth, PropertyNames.BorderBlockStartStyle, PropertyNames.BorderBlockStartColor,
                PropertyNames.BorderBlockEndWidth, PropertyNames.BorderBlockEndStyle, PropertyNames.BorderBlockEndColor);

            var result = expander.Expand(KeywordValue.Inherit);

            Assert.Equal(KeywordValue.Inherit, result[PropertyNames.BorderBlockStartWidth]);
            Assert.Equal(KeywordValue.Inherit, result[PropertyNames.BorderBlockEndStyle]);
            Assert.Equal(KeywordValue.Inherit, result[PropertyNames.BorderBlockEndColor]);
        }
    }
}
