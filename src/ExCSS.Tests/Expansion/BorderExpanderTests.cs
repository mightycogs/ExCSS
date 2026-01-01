using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class BorderExpanderTests
    {
        private readonly BorderExpander _expander = new();

        [Fact]
        public void Expand_Length_SetsWidth()
        {
            var width = new Length(2, Length.Unit.Px);
            var result = _expander.Expand(width);

            Assert.Equal(width, result["border-width"]);
        }

        [Fact]
        public void Expand_Color_SetsColor()
        {
            var color = Color.Red;
            var result = _expander.Expand(color);

            Assert.Equal(color, result["border-color"]);
        }

        [Fact]
        public void Expand_StyleKeyword_SetsStyle()
        {
            var style = new KeywordValue("dashed");
            var result = _expander.Expand(style);

            Assert.Equal(style, result["border-style"]);
        }

        [Fact]
        public void Expand_Var_AssignsColorWhenUnset()
        {
            var varValue = new VarValue("--border-color");
            var result = _expander.Expand(varValue);

            Assert.Equal(varValue, result["border-color"]);
        }

        [Fact]
        public void Expand_GlobalKeyword_PropagatesToAllLonghands()
        {
            var kw = KeywordValue.Inherit;
            var result = _expander.Expand(kw);

            Assert.Equal(kw, result["border-width"]);
            Assert.Equal(kw, result["border-style"]);
            Assert.Equal(kw, result["border-color"]);
        }

        [Fact]
        public void Expand_MultipleValues_SetAll()
        {
            var result = _expander.Expand(new StyleValueTuple(
                new Length(1, Length.Unit.Px),
                Color.Blue,
                new KeywordValue("solid")
            ));

            Assert.Equal("1px", result["border-width"].CssText);
            Assert.Equal("solid", result["border-style"].CssText);
            Assert.Equal(Color.Blue, result["border-color"]);
        }

        [Fact]
        public void Expand_Null_ReturnsEmpty()
        {
            var result = _expander.Expand(null);
            Assert.Empty(result);
        }
    }
}
