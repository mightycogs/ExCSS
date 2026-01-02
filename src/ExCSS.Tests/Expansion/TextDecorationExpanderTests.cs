using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class TextDecorationExpanderTests
    {
        private readonly TextDecorationExpander _expander = new();

        [Fact]
        public void Expand_LineStyleColorThickness()
        {
            var value = new StyleValueTuple(
                new KeywordValue("underline"),
                new KeywordValue("wavy"),
                Color.Red,
                new Length(2, Length.Unit.Px)
            );

            var result = _expander.Expand(value);

            Assert.Equal("underline", result[PropertyNames.TextDecorationLine].CssText);
            Assert.Equal("wavy", result[PropertyNames.TextDecorationStyle].CssText);
            Assert.Equal(Color.Red, result[PropertyNames.TextDecorationColor]);
            Assert.Equal("2px", result[PropertyNames.TextDecorationThickness].CssText);
        }

        [Fact]
        public void Expand_AutoThicknessKeyword()
        {
            var result = _expander.Expand(new StyleValueTuple(
                new KeywordValue("underline"),
                new KeywordValue("auto")
            ));

            Assert.Equal("underline", result[PropertyNames.TextDecorationLine].CssText);
            Assert.Equal("auto", result[PropertyNames.TextDecorationThickness].CssText);
        }

        [Fact]
        public void Expand_GlobalKeyword_Propagates()
        {
            var result = _expander.Expand(KeywordValue.Inherit);
            Assert.All(_expander.LonghandNames, name => Assert.Equal(KeywordValue.Inherit, result[name]));
        }
    }
}
