using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class AnimationExpanderTests
    {
        private readonly AnimationExpander _expander = new();

        [Fact]
        public void Expand_AllFields()
        {
            var value = new StyleValueTuple(
                new KeywordValue("spin"),
                new Time(1f, Time.Unit.S),
                new KeywordValue("linear"),
                new Time(0.5f, Time.Unit.S),
                new Number(2, Number.Unit.Integer),
                new KeywordValue("alternate"),
                new KeywordValue("forwards"),
                new KeywordValue("paused")
            );

            var result = _expander.Expand(value);

            Assert.Equal("spin", result[PropertyNames.AnimationName].CssText);
            Assert.IsType<Time>(result[PropertyNames.AnimationDuration]);
            Assert.Equal("linear", result[PropertyNames.AnimationTimingFunction].CssText);
            var delay = Assert.IsType<Time>(result[PropertyNames.AnimationDelay]);
            Assert.Equal(Time.Unit.S, delay.Type);
            Assert.Equal(0.5f, delay.Value);
            Assert.Equal("2", result[PropertyNames.AnimationIterationCount].CssText);
            Assert.Equal("alternate", result[PropertyNames.AnimationDirection].CssText);
            Assert.Equal("forwards", result[PropertyNames.AnimationFillMode].CssText);
            Assert.Equal("paused", result[PropertyNames.AnimationPlayState].CssText);
        }

        [Fact]
        public void Expand_GlobalKeyword_Propagates()
        {
            var result = _expander.Expand(KeywordValue.Inherit);
            Assert.All(_expander.LonghandNames, name => Assert.Equal(KeywordValue.Inherit, result[name]));
        }
    }
}
