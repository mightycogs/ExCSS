using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class BorderRadiusExpanderTests
    {
        private readonly BorderRadiusExpander _expander = new();

        [Fact]
        public void Expand_OneValue_SetsAllCorners()
        {
            var radius = new Length(5, Length.Unit.Px);
            var result = _expander.Expand(radius);

            Assert.All(_expander.LonghandNames, name => Assert.Equal(radius, result[name]));
        }

        [Fact]
        public void Expand_TwoValues_SetsOppositeCorners()
        {
            var r1 = new Length(5, Length.Unit.Px);
            var r2 = new Length(10, Length.Unit.Px);
            var result = _expander.Expand(new StyleValueTuple(r1, r2));

            Assert.Equal(r1, result["border-top-left-radius"]);
            Assert.Equal(r2, result["border-top-right-radius"]);
            Assert.Equal(r1, result["border-bottom-right-radius"]);
            Assert.Equal(r2, result["border-bottom-left-radius"]);
        }

        [Fact]
        public void Expand_ThreeValues_SetsTopRightBottom()
        {
            var r1 = new Length(1, Length.Unit.Px);
            var r2 = new Length(2, Length.Unit.Px);
            var r3 = new Length(3, Length.Unit.Px);
            var result = _expander.Expand(new StyleValueTuple(r1, r2, r3));

            Assert.Equal(r1, result["border-top-left-radius"]);
            Assert.Equal(r2, result["border-top-right-radius"]);
            Assert.Equal(r3, result["border-bottom-right-radius"]);
            Assert.Equal(r2, result["border-bottom-left-radius"]);
        }

        [Fact]
        public void Expand_FourValues_SetsEachCorner()
        {
            var r1 = new Length(1, Length.Unit.Px);
            var r2 = new Length(2, Length.Unit.Px);
            var r3 = new Length(3, Length.Unit.Px);
            var r4 = new Length(4, Length.Unit.Px);
            var result = _expander.Expand(new StyleValueTuple(r1, r2, r3, r4));

            Assert.Equal(r1, result["border-top-left-radius"]);
            Assert.Equal(r2, result["border-top-right-radius"]);
            Assert.Equal(r3, result["border-bottom-right-radius"]);
            Assert.Equal(r4, result["border-bottom-left-radius"]);
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
