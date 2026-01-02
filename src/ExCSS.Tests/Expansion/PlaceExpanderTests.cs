using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class PlaceExpanderTests
    {
        [Fact]
        public void PlaceContent_SingleValue_AppliesToBoth()
        {
            var expander = new PlaceExpander(PropertyNames.PlaceContent, PropertyNames.AlignContent, PropertyNames.JustifyContent);
            var result = expander.Expand(new KeywordValue("center"));

            Assert.Equal("center", result[PropertyNames.AlignContent].CssText);
            Assert.Equal("center", result[PropertyNames.JustifyContent].CssText);
        }

        [Fact]
        public void PlaceContent_TwoValues_SetsIndividually()
        {
            var expander = new PlaceExpander(PropertyNames.PlaceContent, PropertyNames.AlignContent, PropertyNames.JustifyContent);
            var value = new StyleValueTuple(new KeywordValue("start"), new KeywordValue("space-between"));

            var result = expander.Expand(value);

            Assert.Equal("start", result[PropertyNames.AlignContent].CssText);
            Assert.Equal("space-between", result[PropertyNames.JustifyContent].CssText);
        }

        [Fact]
        public void PlaceItems_PropagatesGlobalKeyword()
        {
            var expander = new PlaceExpander(PropertyNames.PlaceItems, PropertyNames.AlignItems, PropertyNames.JustifyItems);
            var result = expander.Expand(KeywordValue.Inherit);

            Assert.Equal(KeywordValue.Inherit, result[PropertyNames.AlignItems]);
            Assert.Equal(KeywordValue.Inherit, result[PropertyNames.JustifyItems]);
        }

        [Fact]
        public void PlaceSelf_SingleValue_AppliesToBoth()
        {
            var expander = new PlaceExpander(PropertyNames.PlaceSelf, PropertyNames.AlignSelf, PropertyNames.JustifySelf);
            var result = expander.Expand(new KeywordValue("end"));

            Assert.Equal("end", result[PropertyNames.AlignSelf].CssText);
            Assert.Equal("end", result[PropertyNames.JustifySelf].CssText);
        }
    }
}
