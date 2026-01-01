using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class InsetExpanderTests
    {
        private readonly InsetExpander _expander = new();

        [Fact]
        public void Expand_OneValue_AppliesToAllSides()
        {
            var value = new Length(10, Length.Unit.Px);
            var result = _expander.Expand(value);

            Assert.Equal(value, result["top"]);
            Assert.Equal(value, result["right"]);
            Assert.Equal(value, result["bottom"]);
            Assert.Equal(value, result["left"]);
        }

        [Fact]
        public void Expand_TwoValues_SetsVerticalAndHorizontal()
        {
            var vertical = new Length(1, Length.Unit.Px);
            var horizontal = new Length(2, Length.Unit.Px);
            var result = _expander.Expand(new StyleValueTuple(vertical, horizontal));

            Assert.Equal(vertical, result["top"]);
            Assert.Equal(horizontal, result["right"]);
            Assert.Equal(vertical, result["bottom"]);
            Assert.Equal(horizontal, result["left"]);
        }

        [Fact]
        public void Expand_ThreeValues_SetsTopHorizontalBottom()
        {
            var top = new Length(1, Length.Unit.Px);
            var horizontal = new Length(2, Length.Unit.Px);
            var bottom = new Length(3, Length.Unit.Px);
            var result = _expander.Expand(new StyleValueTuple(top, horizontal, bottom));

            Assert.Equal(top, result["top"]);
            Assert.Equal(horizontal, result["right"]);
            Assert.Equal(bottom, result["bottom"]);
            Assert.Equal(horizontal, result["left"]);
        }

        [Fact]
        public void Expand_FourValues_SetsEachSide()
        {
            var top = new Length(1, Length.Unit.Px);
            var right = new Length(2, Length.Unit.Px);
            var bottom = new Length(3, Length.Unit.Px);
            var left = new Length(4, Length.Unit.Px);
            var result = _expander.Expand(new StyleValueTuple(top, right, bottom, left));

            Assert.Equal(top, result["top"]);
            Assert.Equal(right, result["right"]);
            Assert.Equal(bottom, result["bottom"]);
            Assert.Equal(left, result["left"]);
        }

        [Fact]
        public void Expand_GlobalKeyword_Propagates()
        {
            var result = _expander.Expand(KeywordValue.Inherit);

            Assert.All(_expander.LonghandNames, name => Assert.Equal(KeywordValue.Inherit, result[name]));
        }

        [Fact]
        public void Expand_Null_ReturnsEmpty()
        {
            var result = _expander.Expand(null);

            Assert.Empty(result);
        }
    }
}
