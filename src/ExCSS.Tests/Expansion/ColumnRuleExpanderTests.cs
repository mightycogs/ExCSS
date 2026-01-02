using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class ColumnRuleExpanderTests
    {
        private readonly ColumnRuleExpander _expander = new();

        [Fact]
        public void Expand_AllValues_SetsWidthStyleColor()
        {
            var value = new StyleValueTuple(
                new Length(4, Length.Unit.Px),
                new KeywordValue("double"),
                Color.Green
            );

            var result = _expander.Expand(value);

            Assert.Equal("4px", result[PropertyNames.ColumnRuleWidth].CssText);
            Assert.Equal("double", result[PropertyNames.ColumnRuleStyle].CssText);
            Assert.Equal(Color.Green, result[PropertyNames.ColumnRuleColor]);
        }

        [Fact]
        public void Expand_ColorOnly_SetsColor()
        {
            var result = _expander.Expand(Color.Blue);
            Assert.Equal(Color.Blue, result[PropertyNames.ColumnRuleColor]);
        }

        [Fact]
        public void Expand_GlobalKeyword_Propagates()
        {
            var result = _expander.Expand(KeywordValue.Inherit);
            Assert.All(_expander.LonghandNames, name => Assert.Equal(KeywordValue.Inherit, result[name]));
        }
    }
}
