using System.Collections.Generic;
using Xunit;

namespace ExCSS.Tests.Expansion
{
    public class BackgroundExpanderTests
    {
        [Fact]
        public void Expand_SimpleColor_ReturnsColor()
        {
            var expander = new BackgroundExpander();
            var value = new Color(255, 0, 0); // red
            var result = expander.Expand(value);

            Assert.Equal(value, result["background-color"]);
            Assert.Equal("none", result["background-image"].CssText);
        }

        [Fact]
        public void Expand_UrlAndRepeat_ReturnsLonghands()
        {
            var expander = new BackgroundExpander();
            // Parsing "url(a.png) no-repeat" usually produces a Tuple
            var value = new StyleValueTuple(
                new UrlValue("a.png"),
                new KeywordValue("no-repeat")
            );
            
            var result = expander.Expand(value);

            Assert.Equal("url(\"a.png\")", result["background-image"].CssText);
            Assert.Equal("no-repeat", result["background-repeat"].CssText);
            Assert.Equal(Color.Transparent, result["background-color"]);
        }

        [Fact]
        public void Expand_Layers_ReturnsLists()
        {
            var expander = new BackgroundExpander();
            // "url(a.png), url(b.png)" -> List of UrlValues
            var value = new StyleValueList(
                new UrlValue("a.png"),
                new UrlValue("b.png")
            );

            var result = expander.Expand(value);

            var images = result["background-image"] as StyleValueList;
            Assert.NotNull(images);
            Assert.Equal(2, images.Count);
            Assert.Equal("url(\"a.png\")", images[0].CssText);
            Assert.Equal("url(\"b.png\")", images[1].CssText);
        }
    }
}
