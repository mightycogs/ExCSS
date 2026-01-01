using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class BoxModelExpanderTests
    {
        [Fact]
        public void Expand_OneValue_AppliesToAllSides()
        {
            var expander = new BoxModelExpander("margin");
            var value = new Length(10, Length.Unit.Px);

            var result = expander.Expand(value);

            Assert.Equal(value, result["margin-top"]);
            Assert.Equal(value, result["margin-right"]);
            Assert.Equal(value, result["margin-bottom"]);
            Assert.Equal(value, result["margin-left"]);
        }

        [Fact]
        public void Expand_TwoValues_SetsVerticalAndHorizontal()
        {
            var expander = new BoxModelExpander("padding");
            var vertical = new Length(5, Length.Unit.Px);
            var horizontal = new Length(10, Length.Unit.Px);

            var result = expander.Expand(new StyleValueTuple(vertical, horizontal));

            Assert.Equal(vertical, result["padding-top"]);
            Assert.Equal(horizontal, result["padding-right"]);
            Assert.Equal(vertical, result["padding-bottom"]);
            Assert.Equal(horizontal, result["padding-left"]);
        }

        [Fact]
        public void Expand_ThreeValues_SetsTopHorizontalBottom()
        {
            var expander = new BoxModelExpander("margin");
            var top = new Length(1, Length.Unit.Px);
            var horizontal = new Length(2, Length.Unit.Px);
            var bottom = new Length(3, Length.Unit.Px);

            var result = expander.Expand(new StyleValueTuple(top, horizontal, bottom));

            Assert.Equal(top, result["margin-top"]);
            Assert.Equal(horizontal, result["margin-right"]);
            Assert.Equal(bottom, result["margin-bottom"]);
            Assert.Equal(horizontal, result["margin-left"]);
        }

        [Fact]
        public void Expand_FourValues_SetsEachSide()
        {
            var expander = new BoxModelExpander("padding");
            var top = new Length(1, Length.Unit.Px);
            var right = new Length(2, Length.Unit.Px);
            var bottom = new Length(3, Length.Unit.Px);
            var left = new Length(4, Length.Unit.Px);

            var result = expander.Expand(new StyleValueTuple(top, right, bottom, left));

            Assert.Equal(top, result["padding-top"]);
            Assert.Equal(right, result["padding-right"]);
            Assert.Equal(bottom, result["padding-bottom"]);
            Assert.Equal(left, result["padding-left"]);
        }

        [Fact]
        public void Expand_GlobalKeyword_PropagatesToAllLonghands()
        {
            var expander = new BoxModelExpander("margin");
            var result = expander.Expand(KeywordValue.Inherit);

            Assert.All(expander.LonghandNames, name => Assert.Equal(KeywordValue.Inherit, result[name]));
        }

        [Fact]
        public void Expand_Null_ReturnsEmpty()
        {
            var expander = new BoxModelExpander("margin");
            var result = expander.Expand(null);

            Assert.Empty(result);
        }
    }
}
