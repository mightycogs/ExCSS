using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class GapExpanderTests
    {
        private readonly GapExpander _expander = new();

        [Fact]
        public void Expand_SingleValue_AppliesBoth()
        {
            var gap = new Length(10, Length.Unit.Px);
            var result = _expander.Expand(gap);

            Assert.Equal(2, result.Count);
            Assert.Equal(gap, result["row-gap"]);
            Assert.Equal(gap, result["column-gap"]);
        }

        [Fact]
        public void Expand_TwoValues_AppliesRowAndColumn()
        {
            var rowGap = new Length(10, Length.Unit.Px);
            var columnGap = new Length(20, Length.Unit.Px);
            var tuple = new StyleValueTuple(rowGap, columnGap);

            var result = _expander.Expand(tuple);

            Assert.Equal(2, result.Count);
            Assert.Equal(rowGap, result["row-gap"]);
            Assert.Equal(columnGap, result["column-gap"]);
        }

        [Fact]
        public void Expand_Percent_Works()
        {
            var gap = new Percent(5);
            var result = _expander.Expand(gap);

            Assert.Equal(2, result.Count);
            Assert.Equal(gap, result["row-gap"]);
            Assert.Equal(gap, result["column-gap"]);
        }

        [Fact]
        public void Expand_MixedUnits_Works()
        {
            var rowGap = new Length(10, Length.Unit.Px);
            var columnGap = new Percent(5);
            var tuple = new StyleValueTuple(rowGap, columnGap);

            var result = _expander.Expand(tuple);

            Assert.Equal(2, result.Count);
            Assert.Equal(rowGap, result["row-gap"]);
            Assert.Equal(columnGap, result["column-gap"]);
        }

        [Fact]
        public void Expand_Normal_PropagatesKeyword()
        {
            var result = _expander.Expand(new KeywordValue("normal"));

            Assert.Equal(2, result.Count);
            Assert.Equal(new KeywordValue("normal"), result["row-gap"]);
            Assert.Equal(new KeywordValue("normal"), result["column-gap"]);
        }

        [Fact]
        public void Expand_Inherit_PropagatesKeyword()
        {
            var result = _expander.Expand(KeywordValue.Inherit);

            Assert.Equal(2, result.Count);
            Assert.Equal(KeywordValue.Inherit, result["row-gap"]);
            Assert.Equal(KeywordValue.Inherit, result["column-gap"]);
        }

        [Fact]
        public void Expand_Null_ReturnsEmpty()
        {
            var result = _expander.Expand(null);

            Assert.Empty(result);
        }

        [Fact]
        public void LonghandNames_ContainsCorrectProperties()
        {
            Assert.Contains("row-gap", _expander.LonghandNames);
            Assert.Contains("column-gap", _expander.LonghandNames);
            Assert.Equal(2, _expander.LonghandNames.Count);
        }

        [Fact]
        public void ShorthandNames_ContainsGap()
        {
            Assert.Single(_expander.ShorthandNames);
            Assert.Equal("gap", _expander.ShorthandNames[0]);
        }

        [Fact]
        public void Expand_ThreeValues_ReturnsEmpty()
        {
            var tuple = new StyleValueTuple(
                new Length(10, Length.Unit.Px),
                new Length(20, Length.Unit.Px),
                new Length(30, Length.Unit.Px)
            );

            var result = _expander.Expand(tuple);

            Assert.Empty(result);
        }
    }
}
