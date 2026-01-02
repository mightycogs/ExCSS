using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class ListStyleExpanderTests
    {
        private readonly ListStyleExpander _expander = new();

        [Fact]
        public void Expand_TypePositionImage_OrderAgnostic()
        {
            var value = new StyleValueTuple(
                new KeywordValue("square"),
                new KeywordValue("inside"),
                new UrlValue("bullet.png")
            );

            var result = _expander.Expand(value);

            Assert.Equal("square", result[PropertyNames.ListStyleType].CssText);
            Assert.Equal("inside", result[PropertyNames.ListStylePosition].CssText);
            Assert.Equal("url(\"bullet.png\")", result[PropertyNames.ListStyleImage].CssText);
        }

        [Fact]
        public void Expand_None_SetsTypeAndImageNone()
        {
            var result = _expander.Expand(KeywordValue.None);

            Assert.Equal("none", result[PropertyNames.ListStyleType].CssText);
            Assert.Equal("none", result[PropertyNames.ListStyleImage].CssText);
            Assert.Equal("outside", result[PropertyNames.ListStylePosition].CssText);
        }

        [Fact]
        public void Expand_UrlOnly_DefaultsTypeAndPosition()
        {
            var result = _expander.Expand(new UrlValue("bullet.png"));

            Assert.Equal("disc", result[PropertyNames.ListStyleType].CssText);
            Assert.Equal("outside", result[PropertyNames.ListStylePosition].CssText);
            Assert.Equal("url(\"bullet.png\")", result[PropertyNames.ListStyleImage].CssText);
        }

        [Fact]
        public void Expand_TypeAndPosition_DefaultsImage()
        {
            var value = new StyleValueTuple(new KeywordValue("circle"), new KeywordValue("inside"));
            var result = _expander.Expand(value);

            Assert.Equal("circle", result[PropertyNames.ListStyleType].CssText);
            Assert.Equal("inside", result[PropertyNames.ListStylePosition].CssText);
            Assert.Equal("none", result[PropertyNames.ListStyleImage].CssText);
        }

        [Fact]
        public void Expand_GlobalKeyword_Propagates()
        {
            var result = _expander.Expand(KeywordValue.Inherit);

            Assert.All(_expander.LonghandNames, name => Assert.Equal(KeywordValue.Inherit, result[name]));
        }
    }
}
