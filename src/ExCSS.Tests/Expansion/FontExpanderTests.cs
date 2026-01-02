using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class FontExpanderTests
    {
        private readonly FontExpander _expander = new();

        [Fact]
        public void Expand_StyleWeightSizeFamily()
        {
            var value = new StyleValueTuple(
                new KeywordValue("italic"),
                new KeywordValue("700"),
                new Length(16, Length.Unit.Px),
                new KeywordValue("Arial")
            );

            var result = _expander.Expand(value);

            Assert.Equal("italic", result[PropertyNames.FontStyle].CssText);
            Assert.Equal("700", result[PropertyNames.FontWeight].CssText);
            Assert.Equal("16px", result[PropertyNames.FontSize].CssText);
            Assert.Equal("Arial", result[PropertyNames.FontFamily].CssText);
        }

        [Fact]
        public void Expand_SizeLineHeightFamily()
        {
            var value = new StyleValueTuple(
                new Length(14, Length.Unit.Px),
                new RawValue("/"),
                new Number(1.4f, Number.Unit.Float),
                new KeywordValue("serif")
            );

            var result = _expander.Expand(value);

            Assert.Equal("14px", result[PropertyNames.FontSize].CssText);
            var lh = Assert.IsType<Number>(result[PropertyNames.LineHeight]);
            Assert.InRange(lh.Value, 1.39, 1.41);
            Assert.Equal("serif", result[PropertyNames.FontFamily].CssText);
        }

        [Fact]
        public void Expand_GlobalKeyword_Propagates()
        {
            var result = _expander.Expand(KeywordValue.Inherit);
            Assert.All(_expander.LonghandNames, name => Assert.Equal(KeywordValue.Inherit, result[name]));
        }
    }
}
