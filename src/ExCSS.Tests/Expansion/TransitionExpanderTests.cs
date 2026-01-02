using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class TransitionExpanderTests
    {
        private readonly TransitionExpander _expander = new();

        [Fact]
        public void Expand_AllFields()
        {
            var value = new StyleValueTuple(
                new KeywordValue("opacity"),
                new Time(200, Time.Unit.Ms),
                new KeywordValue("ease-in"),
                new Time(50, Time.Unit.Ms)
            );

            var result = _expander.Expand(value);

            Assert.Equal("opacity", result[PropertyNames.TransitionProperty].CssText);
            Assert.Equal("200ms", result[PropertyNames.TransitionDuration].CssText);
            Assert.Equal("ease-in", result[PropertyNames.TransitionTimingFunction].CssText);
            Assert.Equal("50ms", result[PropertyNames.TransitionDelay].CssText);
        }

        [Fact]
        public void Expand_GlobalKeyword_Propagates()
        {
            var result = _expander.Expand(KeywordValue.Inherit);
            Assert.All(_expander.LonghandNames, name => Assert.Equal(KeywordValue.Inherit, result[name]));
        }
    }
}
