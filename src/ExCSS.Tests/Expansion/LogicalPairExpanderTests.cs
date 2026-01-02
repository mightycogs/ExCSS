using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class LogicalPairExpanderTests
    {
        [Fact]
        public void MarginInline_OneValue_AppliesToStartEnd()
        {
            var expander = new LogicalPairExpander(PropertyNames.MarginInline, PropertyNames.MarginInlineStart, PropertyNames.MarginInlineEnd);
            var result = expander.Expand(new Length(10, Length.Unit.Px));

            Assert.Equal("10px", result[PropertyNames.MarginInlineStart].CssText);
            Assert.Equal("10px", result[PropertyNames.MarginInlineEnd].CssText);
        }

        [Fact]
        public void PaddingBlock_TwoValues_SetsStartEnd()
        {
            var expander = new LogicalPairExpander(PropertyNames.PaddingBlock, PropertyNames.PaddingBlockStart, PropertyNames.PaddingBlockEnd);
            var result = expander.Expand(new StyleValueTuple(
                new Length(5, Length.Unit.Px),
                new Length(8, Length.Unit.Px)
            ));

            Assert.Equal("5px", result[PropertyNames.PaddingBlockStart].CssText);
            Assert.Equal("8px", result[PropertyNames.PaddingBlockEnd].CssText);
        }

        [Fact]
        public void MarginInline_GlobalKeyword_Propagates()
        {
            var expander = new LogicalPairExpander(PropertyNames.MarginInline, PropertyNames.MarginInlineStart, PropertyNames.MarginInlineEnd);
            var result = expander.Expand(KeywordValue.Inherit);

            Assert.Equal(KeywordValue.Inherit, result[PropertyNames.MarginInlineStart]);
            Assert.Equal(KeywordValue.Inherit, result[PropertyNames.MarginInlineEnd]);
        }
    }
}
