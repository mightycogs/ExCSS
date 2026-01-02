using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class BorderSideExpanderTests
    {
        [Fact]
        public void BorderTop_ExpandsWidthStyleColor()
        {
            var expander = new BorderSideExpander(
                PropertyNames.BorderTop,
                PropertyNames.BorderTopWidth,
                PropertyNames.BorderTopStyle,
                PropertyNames.BorderTopColor);

            var result = expander.Expand(new StyleValueTuple(
                new Length(3, Length.Unit.Px),
                new KeywordValue("solid"),
                Color.Blue
            ));

            Assert.Equal("3px", result[PropertyNames.BorderTopWidth].CssText);
            Assert.Equal("solid", result[PropertyNames.BorderTopStyle].CssText);
            Assert.Equal(Color.Blue, result[PropertyNames.BorderTopColor]);
        }

        [Fact]
        public void BorderLeft_GlobalKeyword_Propagates()
        {
            var expander = new BorderSideExpander(
                PropertyNames.BorderLeft,
                PropertyNames.BorderLeftWidth,
                PropertyNames.BorderLeftStyle,
                PropertyNames.BorderLeftColor);

            var result = expander.Expand(KeywordValue.Inherit);

            Assert.Equal(KeywordValue.Inherit, result[PropertyNames.BorderLeftWidth]);
            Assert.Equal(KeywordValue.Inherit, result[PropertyNames.BorderLeftStyle]);
            Assert.Equal(KeywordValue.Inherit, result[PropertyNames.BorderLeftColor]);
        }
    }
}
