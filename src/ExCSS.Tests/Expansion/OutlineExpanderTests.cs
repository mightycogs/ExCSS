using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class OutlineExpanderTests
    {
        private readonly OutlineExpander _expander = new();

        [Fact]
        public void Expand_AllThreeValues_SetsWidthStyleColor()
        {
            var value = new StyleValueTuple(
                new Length(2, Length.Unit.Px),
                new KeywordValue("dashed"),
                Color.Red
            );

            var result = _expander.Expand(value);

            Assert.Equal("2px", result[PropertyNames.OutlineWidth].CssText);
            Assert.Equal("dashed", result[PropertyNames.OutlineStyle].CssText);
            Assert.Equal(Color.Red, result[PropertyNames.OutlineColor]);
        }

        [Fact]
        public void Expand_StyleOnly_SetsStyle()
        {
            var result = _expander.Expand(new KeywordValue("solid"));

            Assert.Equal("solid", result[PropertyNames.OutlineStyle].CssText);
        }

        [Fact]
        public void Expand_ColorOnly_SetsColor()
        {
            var result = _expander.Expand(Color.Blue);

            Assert.Equal(Color.Blue, result[PropertyNames.OutlineColor]);
        }

        [Fact]
        public void Expand_LengthOnly_SetsWidth()
        {
            var result = _expander.Expand(new Length(1, Length.Unit.Px));

            Assert.Equal("1px", result[PropertyNames.OutlineWidth].CssText);
        }

        [Fact]
        public void Expand_GlobalKeyword_Propagates()
        {
            var result = _expander.Expand(KeywordValue.Inherit);

            Assert.All(_expander.LonghandNames, name => Assert.Equal(KeywordValue.Inherit, result[name]));
        }
    }
}
