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

        [Fact]
        public void Expand_RepeatWithTwoValues_KeepsBothDirections()
        {
            var expander = new BackgroundExpander();
            var value = new StyleValueTuple(
                new UrlValue("a.png"),
                new KeywordValue("repeat"),
                new KeywordValue("no-repeat")
            );

            var result = expander.Expand(value);
            var repeat = Assert.IsType<StyleValueTuple>(result["background-repeat"]);
            Assert.Equal("repeat", repeat[0].CssText);
            Assert.Equal("no-repeat", repeat[1].CssText);
        }

        [Fact]
        public void Expand_RepeatX_ExpandsToRepeatNoRepeat()
        {
            var expander = new BackgroundExpander();
            var value = new StyleValueTuple(
                new UrlValue("a.png"),
                new KeywordValue("repeat-x")
            );

            var result = expander.Expand(value);
            var repeat = Assert.IsType<StyleValueTuple>(result["background-repeat"]);
            Assert.Equal("repeat", repeat[0].CssText);
            Assert.Equal("no-repeat", repeat[1].CssText);
        }

        [Fact]
        public void Expand_SinglePosition_DefaultsMissingAxisToCenter()
        {
            var expander = new BackgroundExpander();
            var value = new StyleValueTuple(
                new UrlValue("a.png"),
                new KeywordValue("top")
            );

            var result = expander.Expand(value);
            var position = Assert.IsType<StyleValueTuple>(result["background-position"]);
            Assert.Equal("50%", position[0].CssText);
            Assert.Equal("top", position[1].CssText);
        }

        [Fact]
        public void Expand_OriginAndClip_AreSeparated()
        {
            var expander = new BackgroundExpander();
            var value = new StyleValueTuple(
                new UrlValue("a.png"),
                new KeywordValue("padding-box"),
                new KeywordValue("content-box")
            );

            var result = expander.Expand(value);
            Assert.Equal("padding-box", result["background-origin"].CssText);
            Assert.Equal("content-box", result["background-clip"].CssText);
        }

        [Fact]
        public void Expand_GlobalKeyword_PropagatesToAllLonghands()
        {
            var expander = new BackgroundExpander();
            var value = new KeywordValue("inherit");

            var result = expander.Expand(value);
            Assert.All(BackgroundLonghands, name => Assert.Equal(value, result[name]));
        }

        private static readonly string[] BackgroundLonghands =
        {
            PropertyNames.BackgroundColor,
            PropertyNames.BackgroundImage,
            PropertyNames.BackgroundPosition,
            PropertyNames.BackgroundSize,
            PropertyNames.BackgroundRepeat,
            PropertyNames.BackgroundAttachment,
            PropertyNames.BackgroundOrigin,
            PropertyNames.BackgroundClip
        };

        [Fact]
        public void Expand_PositionAndSize_WithSlash_SetsBoth()
        {
            var expander = new BackgroundExpander();
            // "left / contain"
            var value = new StyleValueTuple(
                new KeywordValue("left"),
                new RawValue("/"),
                new KeywordValue("contain")
            );

            var result = expander.Expand(value);
            var position = Assert.IsType<StyleValueTuple>(result["background-position"]);
            Assert.Equal("left", position[0].CssText);
            Assert.Equal("50%", position[1].CssText);

            Assert.Equal("contain", result["background-size"].CssText);
        }

        [Fact]
        public void Expand_SingleBoxKeyword_SetsOriginAndClip()
        {
            var expander = new BackgroundExpander();
            var value = new StyleValueTuple(
                new UrlValue("a.png"),
                new KeywordValue("padding-box")
            );

            var result = expander.Expand(value);
            Assert.Equal("padding-box", result["background-origin"].CssText);
            Assert.Equal("padding-box", result["background-clip"].CssText);
        }

        [Fact]
        public void Expand_MixedLayers_AlignsLists()
        {
            var expander = new BackgroundExpander();
            var value = new StyleValueList(
                new StyleValueTuple(new UrlValue("a.png"), new KeywordValue("left"), new RawValue("/"), new KeywordValue("auto")),
                new StyleValueTuple(new UrlValue("b.png"), new KeywordValue("center"))
            );

            var result = expander.Expand(value);

            var positions = Assert.IsType<StyleValueList>(result["background-position"]);
            Assert.Equal(2, positions.Count);
            Assert.Equal("left 50%", positions[0].CssText);
            Assert.Equal("center center", positions[1].CssText);

            var sizes = Assert.IsType<StyleValueList>(result["background-size"]);
            Assert.Equal(2, sizes.Count);
            Assert.Equal("auto", sizes[0].CssText);
            Assert.Equal("auto", sizes[1].CssText);
        }
    }
}
