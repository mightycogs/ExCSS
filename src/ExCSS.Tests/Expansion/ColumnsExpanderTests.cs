using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class ColumnsExpanderTests
    {
        private readonly ColumnsExpander _expander = new();

        [Fact]
        public void Expand_LengthAndNumber_SetsWidthAndCount()
        {
            var value = new StyleValueTuple(
                new Length(200, Length.Unit.Px),
                new Number(3, Number.Unit.Integer)
            );

            var result = _expander.Expand(value);

            Assert.Equal("200px", result[PropertyNames.ColumnWidth].CssText);
            Assert.Equal("3", result[PropertyNames.ColumnCount].CssText);
        }

        [Fact]
        public void Expand_NumberOnly_SetsCountAndAutoWidth()
        {
            var result = _expander.Expand(new Number(2, Number.Unit.Integer));

            Assert.Equal("auto", result[PropertyNames.ColumnWidth].CssText);
            Assert.Equal("2", result[PropertyNames.ColumnCount].CssText);
        }

        [Fact]
        public void Expand_LengthOnly_SetsWidthAndAutoCount()
        {
            var result = _expander.Expand(new Length(150, Length.Unit.Px));

            Assert.Equal("150px", result[PropertyNames.ColumnWidth].CssText);
            Assert.Equal("auto", result[PropertyNames.ColumnCount].CssText);
        }

        [Fact]
        public void Expand_GlobalKeyword_Propagates()
        {
            var result = _expander.Expand(KeywordValue.Inherit);

            Assert.Equal(KeywordValue.Inherit, result[PropertyNames.ColumnWidth]);
            Assert.Equal(KeywordValue.Inherit, result[PropertyNames.ColumnCount]);
        }
    }
}
