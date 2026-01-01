using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class FlexExpanderTests
    {
        private readonly FlexExpander _expander = new();

        [Fact]
        public void Expand_None_ReturnsZeroZeroAuto()
        {
            var result = _expander.Expand(KeywordValue.None);

            Assert.Equal(3, result.Count);
            Assert.Equal(Number.Zero, result["flex-grow"]);
            Assert.Equal(Number.Zero, result["flex-shrink"]);
            Assert.Equal(KeywordValue.Auto, result["flex-basis"]);
        }

        [Fact]
        public void Expand_Auto_ReturnsOneOneAuto()
        {
            var result = _expander.Expand(KeywordValue.Auto);

            Assert.Equal(3, result.Count);
            Assert.Equal(Number.One, result["flex-grow"]);
            Assert.Equal(Number.One, result["flex-shrink"]);
            Assert.Equal(KeywordValue.Auto, result["flex-basis"]);
        }

        [Fact]
        public void Expand_Initial_ReturnsZeroOneAuto()
        {
            var result = _expander.Expand(KeywordValue.Initial);

            Assert.Equal(3, result.Count);
            Assert.Equal(Number.Zero, result["flex-grow"]);
            Assert.Equal(Number.One, result["flex-shrink"]);
            Assert.Equal(KeywordValue.Auto, result["flex-basis"]);
        }

        [Fact]
        public void Expand_SingleNumber_SetsBasisToZero()
        {
            var result = _expander.Expand(new Number(2, Number.Unit.Integer));

            Assert.Equal(3, result.Count);
            Assert.Equal(new Number(2, Number.Unit.Integer), result["flex-grow"]);
            Assert.Equal(Number.One, result["flex-shrink"]);
            Assert.Equal(Number.Zero, result["flex-basis"]);
        }

        [Fact]
        public void Expand_TwoNumbers_SetsGrowAndShrink()
        {
            var tuple = new StyleValueTuple(
                new Number(2, Number.Unit.Integer),
                new Number(3, Number.Unit.Integer)
            );

            var result = _expander.Expand(tuple);

            Assert.Equal(3, result.Count);
            Assert.Equal(new Number(2, Number.Unit.Integer), result["flex-grow"]);
            Assert.Equal(new Number(3, Number.Unit.Integer), result["flex-shrink"]);
            Assert.Equal(Number.Zero, result["flex-basis"]);
        }

        [Fact]
        public void Expand_NumberAndLength_SetsGrowAndBasis()
        {
            var tuple = new StyleValueTuple(
                new Number(1, Number.Unit.Integer),
                new Length(100, Length.Unit.Px)
            );

            var result = _expander.Expand(tuple);

            Assert.Equal(3, result.Count);
            Assert.Equal(new Number(1, Number.Unit.Integer), result["flex-grow"]);
            Assert.Equal(Number.One, result["flex-shrink"]);
            Assert.Equal(new Length(100, Length.Unit.Px), result["flex-basis"]);
        }

        [Fact]
        public void Expand_LengthOnly_SetsGrowToOneAndBasis()
        {
            var result = _expander.Expand(new Length(50, Length.Unit.Percent));

            Assert.Equal(3, result.Count);
            Assert.Equal(Number.One, result["flex-grow"]);
            Assert.Equal(Number.One, result["flex-shrink"]);
            Assert.Equal(new Length(50, Length.Unit.Percent), result["flex-basis"]);
        }

        [Fact]
        public void Expand_Inherit_PropagatesKeyword()
        {
            var result = _expander.Expand(KeywordValue.Inherit);

            Assert.Equal(3, result.Count);
            Assert.Equal(KeywordValue.Inherit, result["flex-grow"]);
            Assert.Equal(KeywordValue.Inherit, result["flex-shrink"]);
            Assert.Equal(KeywordValue.Inherit, result["flex-basis"]);
        }

        [Fact]
        public void Expand_ThreeValues_SetsAllLonghands()
        {
            var tuple = new StyleValueTuple(
                new Number(2, Number.Unit.Integer),
                new Number(3, Number.Unit.Integer),
                new Length(200, Length.Unit.Px)
            );

            var result = _expander.Expand(tuple);

            Assert.Equal(3, result.Count);
            Assert.Equal(new Number(2, Number.Unit.Integer), result["flex-grow"]);
            Assert.Equal(new Number(3, Number.Unit.Integer), result["flex-shrink"]);
            Assert.Equal(new Length(200, Length.Unit.Px), result["flex-basis"]);
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
            Assert.Contains("flex-grow", _expander.LonghandNames);
            Assert.Contains("flex-shrink", _expander.LonghandNames);
            Assert.Contains("flex-basis", _expander.LonghandNames);
            Assert.Equal(3, _expander.LonghandNames.Count);
        }

        [Fact]
        public void ShorthandNames_ContainsFlex()
        {
            Assert.Single(_expander.ShorthandNames);
            Assert.Equal("flex", _expander.ShorthandNames[0]);
        }

        [Fact]
        public void Expand_NegativeGrow_ReturnsEmpty()
        {
            var result = _expander.Expand(new Number(-1, Number.Unit.Integer));

            Assert.Empty(result);
        }

        [Fact]
        public void Expand_NegativeShrink_ReturnsEmpty()
        {
            var tuple = new StyleValueTuple(
                new Number(1, Number.Unit.Integer),
                new Number(-2, Number.Unit.Integer)
            );

            var result = _expander.Expand(tuple);

            Assert.Empty(result);
        }

        [Fact]
        public void Expand_ContentKeyword_RecognizedAsBasis()
        {
            var tuple = new StyleValueTuple(
                new Number(1, Number.Unit.Integer),
                new KeywordValue("content")
            );

            var result = _expander.Expand(tuple);

            Assert.Equal(3, result.Count);
            Assert.Equal(new Number(1, Number.Unit.Integer), result["flex-grow"]);
            Assert.Equal(new KeywordValue("content"), result["flex-basis"]);
        }

        [Fact]
        public void Expand_MinContentKeyword_RecognizedAsBasis()
        {
            var result = _expander.Expand(new KeywordValue("min-content"));

            Assert.Equal(3, result.Count);
            Assert.Equal(Number.One, result["flex-grow"]);
            Assert.Equal(new KeywordValue("min-content"), result["flex-basis"]);
        }

        [Fact]
        public void Expand_MaxContentKeyword_RecognizedAsBasis()
        {
            var result = _expander.Expand(new KeywordValue("max-content"));

            Assert.Equal(3, result.Count);
            Assert.Equal(Number.One, result["flex-grow"]);
            Assert.Equal(new KeywordValue("max-content"), result["flex-basis"]);
        }
    }
}
