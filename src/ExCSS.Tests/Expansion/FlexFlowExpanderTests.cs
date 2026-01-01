using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class FlexFlowExpanderTests
    {
        private readonly FlexFlowExpander _expander = new();

        [Fact]
        public void Expand_Row_SetsDirectionOnly()
        {
            var result = _expander.Expand(new KeywordValue("row"));

            Assert.Equal(2, result.Count);
            Assert.Equal(new KeywordValue("row"), result["flex-direction"]);
            Assert.Equal(new KeywordValue("nowrap"), result["flex-wrap"]);
        }

        [Fact]
        public void Expand_Column_SetsDirectionOnly()
        {
            var result = _expander.Expand(new KeywordValue("column"));

            Assert.Equal(2, result.Count);
            Assert.Equal(new KeywordValue("column"), result["flex-direction"]);
            Assert.Equal(new KeywordValue("nowrap"), result["flex-wrap"]);
        }

        [Fact]
        public void Expand_Wrap_SetsWrapOnly()
        {
            var result = _expander.Expand(new KeywordValue("wrap"));

            Assert.Equal(2, result.Count);
            Assert.Equal(new KeywordValue("row"), result["flex-direction"]);
            Assert.Equal(new KeywordValue("wrap"), result["flex-wrap"]);
        }

        [Fact]
        public void Expand_RowWrap_SetsBoth()
        {
            var tuple = new StyleValueTuple(
                new KeywordValue("row"),
                new KeywordValue("wrap")
            );

            var result = _expander.Expand(tuple);

            Assert.Equal(2, result.Count);
            Assert.Equal(new KeywordValue("row"), result["flex-direction"]);
            Assert.Equal(new KeywordValue("wrap"), result["flex-wrap"]);
        }

        [Fact]
        public void Expand_ColumnReverse_WrapReverse_SetsBoth()
        {
            var tuple = new StyleValueTuple(
                new KeywordValue("column-reverse"),
                new KeywordValue("wrap-reverse")
            );

            var result = _expander.Expand(tuple);

            Assert.Equal(2, result.Count);
            Assert.Equal(new KeywordValue("column-reverse"), result["flex-direction"]);
            Assert.Equal(new KeywordValue("wrap-reverse"), result["flex-wrap"]);
        }

        [Fact]
        public void Expand_WrapRow_OrderDoesNotMatter()
        {
            var tuple = new StyleValueTuple(
                new KeywordValue("wrap"),
                new KeywordValue("row-reverse")
            );

            var result = _expander.Expand(tuple);

            Assert.Equal(2, result.Count);
            Assert.Equal(new KeywordValue("row-reverse"), result["flex-direction"]);
            Assert.Equal(new KeywordValue("wrap"), result["flex-wrap"]);
        }

        [Fact]
        public void Expand_Inherit_PropagatesKeyword()
        {
            var result = _expander.Expand(KeywordValue.Inherit);

            Assert.Equal(2, result.Count);
            Assert.Equal(KeywordValue.Inherit, result["flex-direction"]);
            Assert.Equal(KeywordValue.Inherit, result["flex-wrap"]);
        }

        [Fact]
        public void Expand_Initial_PropagatesKeyword()
        {
            var result = _expander.Expand(KeywordValue.Initial);

            Assert.Equal(2, result.Count);
            Assert.Equal(KeywordValue.Initial, result["flex-direction"]);
            Assert.Equal(KeywordValue.Initial, result["flex-wrap"]);
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
            Assert.Contains("flex-direction", _expander.LonghandNames);
            Assert.Contains("flex-wrap", _expander.LonghandNames);
            Assert.Equal(2, _expander.LonghandNames.Count);
        }

        [Fact]
        public void ShorthandNames_ContainsFlexFlow()
        {
            Assert.Single(_expander.ShorthandNames);
            Assert.Equal("flex-flow", _expander.ShorthandNames[0]);
        }

        [Fact]
        public void Expand_DuplicateDirection_ReturnsEmpty()
        {
            var tuple = new StyleValueTuple(
                new KeywordValue("row"),
                new KeywordValue("column")
            );

            var result = _expander.Expand(tuple);

            Assert.Empty(result);
        }

        [Fact]
        public void Expand_DuplicateWrap_ReturnsEmpty()
        {
            var tuple = new StyleValueTuple(
                new KeywordValue("wrap"),
                new KeywordValue("nowrap")
            );

            var result = _expander.Expand(tuple);

            Assert.Empty(result);
        }
    }
}
