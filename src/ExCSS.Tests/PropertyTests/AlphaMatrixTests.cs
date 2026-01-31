using System.Linq;
using ExCSS.Tests.Helpers;
using Xunit;

namespace ExCSS.Tests.PropertyTests
{
    /// <summary>
    /// Tests based on test_alpha_matrix.html - checkerboard pattern with 4 gradient layers.
    /// Verifies all multi-layer gradients are correctly parsed and returned.
    /// Uses ParseFailureDetector to catch silent fallbacks to RawValue.
    /// </summary>
    public class AlphaMatrixTests
    {
        private readonly StylesheetParser _parser = new();

        // CSS from test_alpha_matrix.html - checkerboard pattern with 4 gradients
        private const string CheckerboardCss = @"
            .background-check {
                position: fixed;
                top: 0; left: 0; width: 100%; height: 100%;
                z-index: 0;

                background-image:
                    linear-gradient(45deg, #ccc 25%, transparent 25%),
                    linear-gradient(-45deg, #ccc 25%, transparent 25%),
                    linear-gradient(45deg, transparent 75%, #ccc 75%),
                    linear-gradient(-45deg, transparent 75%, #ccc 75%);
                background-size: 20px 20px;
                background-color: #fff;
            }";

        // CSS from test_alpha_matrix.html - overlay with alpha gradient
        private const string OverlayCss = @"
            .background-overlay {
                position: fixed;
                top: 0; left: 0; width: 100%; height: 100%;
                z-index: 1;
                background: linear-gradient(135deg, rgba(0,0,0,0.9) 0%, rgba(0,0,0,0.2) 50%, rgba(255,0,0,0.5) 100%);
            }";

        [Fact]
        public void CheckerboardPattern_ShouldHaveFourGradientLayers()
        {
            var sheet = _parser.Parse(CheckerboardCss);
            var rule = sheet.StyleRules.First();

            var bgImageProp = rule.Style.GetProperty("background-image");

            Assert.NotNull(bgImageProp);
            Assert.True(bgImageProp.HasValue, "background-image should have a value");
            ParseFailureDetector.AssertNoParseFailure(bgImageProp);

            var list = Assert.IsType<StyleValueList>(bgImageProp.TypedValue);
            Assert.Equal(4, list.Count);
            Assert.All(list, item => Assert.IsType<LinearGradient>(item));
        }

        [Fact]
        public void CheckerboardPattern_AllAnglesPresent()
        {
            var sheet = _parser.Parse(CheckerboardCss);
            var rule = sheet.StyleRules.First();

            var bgImageProp = rule.Style.GetProperty("background-image");
            ParseFailureDetector.AssertNoParseFailure(bgImageProp);

            var list = Assert.IsType<StyleValueList>(bgImageProp.TypedValue);
            var gradients = list.OfType<LinearGradient>().ToList();

            Assert.Equal(4, gradients.Count);

            var angles = gradients.Select(g => g.Angle).ToList();

            Assert.Equal(2, angles.Count(a => a.Value == 45 && a.Type == Angle.Unit.Deg));
            Assert.Equal(2, angles.Count(a => a.Value == -45 && a.Type == Angle.Unit.Deg));
        }

        [Fact]
        public void CheckerboardPattern_HasBackgroundColor()
        {
            var sheet = _parser.Parse(CheckerboardCss);
            var rule = sheet.StyleRules.First();

            var bgColorProp = rule.Style.GetProperty("background-color");

            Assert.NotNull(bgColorProp);
            Assert.True(bgColorProp.HasValue, "background-color should have a value");
            ParseFailureDetector.AssertNoParseFailure(bgColorProp);
            Assert.Equal("rgb(255, 255, 255)", bgColorProp.Value);
        }

        [Fact]
        public void CheckerboardPattern_HasBackgroundSize()
        {
            var sheet = _parser.Parse(CheckerboardCss);
            var rule = sheet.StyleRules.First();

            var bgSizeProp = rule.Style.GetProperty("background-size");

            Assert.NotNull(bgSizeProp);
            Assert.True(bgSizeProp.HasValue);
            ParseFailureDetector.AssertNoParseFailure(bgSizeProp);
            Assert.Contains("20px", bgSizeProp.Value);
        }

        [Fact]
        public void CheckerboardPattern_TypedValue_ReturnsFourGradients()
        {
            var sheet = _parser.Parse(CheckerboardCss);
            var rule = sheet.StyleRules.First();

            var bgImageProp = rule.Style.GetProperty("background-image");
            ParseFailureDetector.AssertNoParseFailure(bgImageProp);

            var typedValue = bgImageProp.TypedValue;
            Assert.NotNull(typedValue);

            var list = Assert.IsType<StyleValueList>(typedValue);
            Assert.Equal(4, list.Count);

            foreach (var item in list)
            {
                Assert.True(item is LinearGradient, $"Expected LinearGradient, got {item?.GetType().Name}");
            }
        }

        [Fact]
        public void OverlayGradient_ParsesAlphaColors()
        {
            var sheet = _parser.Parse(OverlayCss);
            var rule = sheet.StyleRules.First();

            var bgImageProp = rule.Style.GetProperty("background-image");

            Assert.NotNull(bgImageProp);
            Assert.True(bgImageProp.HasValue);
            ParseFailureDetector.AssertNoParseFailure(bgImageProp);

            var gradient = Assert.IsType<LinearGradient>(bgImageProp.TypedValue);

            Assert.Equal(135, gradient.Angle.Value);
            Assert.Equal(Angle.Unit.Deg, gradient.Angle.Type);

            var stops = gradient.Stops.ToList();
            Assert.All(stops, s => Assert.True(s.Color.Alpha < 1.0, "Expected rgba colors with alpha < 1"));
        }

        [Fact]
        public void OverlayGradient_PreservesAlphaValues()
        {
            var sheet = _parser.Parse(OverlayCss);
            var rule = sheet.StyleRules.First();

            var bgImageProp = rule.Style.GetProperty("background-image");
            ParseFailureDetector.AssertNoParseFailure(bgImageProp);

            var gradient = Assert.IsType<LinearGradient>(bgImageProp.TypedValue);
            var stops = gradient.Stops.ToList();

            Assert.Equal(0.9, stops[0].Color.Alpha, 1);
            Assert.Equal(0.2, stops[1].Color.Alpha, 1);
            Assert.Equal(0.5, stops[2].Color.Alpha, 1);
        }

        [Fact]
        public void OverlayGradient_TypedValue_HasCorrectAlphaInStops()
        {
            var sheet = _parser.Parse(OverlayCss);
            var rule = sheet.StyleRules.First();

            var bgImageProp = rule.Style.GetProperty("background-image");
            ParseFailureDetector.AssertNoParseFailure(bgImageProp);

            var gradient = Assert.IsType<LinearGradient>(bgImageProp.TypedValue);

            var stops = gradient.Stops.ToList();
            Assert.Equal(3, stops.Count);

            Assert.Equal(0.9, stops[0].Color.Alpha, 1);
            Assert.Equal(0.2, stops[1].Color.Alpha, 1);
            Assert.Equal(0.5, stops[2].Color.Alpha, 1);
            Assert.Equal(255, stops[2].Color.R);
        }

        [Fact]
        public void FullAlphaMatrixCss_AllRulesParse()
        {
            var css = @"
                body {
                    margin: 0;
                    padding: 0;
                    height: 100vh;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    background-color: #111;
                    font-family: monospace;
                    color: white;
                }

                .background-check {
                    position: fixed;
                    top: 0; left: 0; width: 100%; height: 100%;
                    z-index: 0;
                    background-image:
                        linear-gradient(45deg, #ccc 25%, transparent 25%),
                        linear-gradient(-45deg, #ccc 25%, transparent 25%),
                        linear-gradient(45deg, transparent 75%, #ccc 75%),
                        linear-gradient(-45deg, transparent 75%, #ccc 75%);
                    background-size: 20px 20px;
                    background-color: #fff;
                }

                .background-overlay {
                    position: fixed;
                    top: 0; left: 0; width: 100%; height: 100%;
                    z-index: 1;
                    background: linear-gradient(135deg, rgba(0,0,0,0.9) 0%, rgba(0,0,0,0.2) 50%, rgba(255,0,0,0.5) 100%);
                }

                .ui-container {
                    position: relative;
                    z-index: 2;
                    background: rgba(0,0,0,0.8);
                    padding: 20px;
                    border-radius: 10px;
                    border: 1px solid #555;
                    box-shadow: 0 10px 30px rgba(0,0,0,0.5);
                }

                .cell {
                    width: 100%;
                    height: 100%;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    border-radius: 4px;
                    font-size: 10px;
                    text-shadow: 0 1px 2px rgba(0,0,0,0.8);
                    position: relative;
                }

                .inner-dot {
                    width: 20px;
                    height: 20px;
                    border-radius: 50%;
                    box-shadow: 0 2px 4px rgba(0,0,0,0.3);
                }
            ";

            var sheet = _parser.Parse(css);

            Assert.Equal(6, sheet.StyleRules.Count());

            var bgCheckRule = sheet.StyleRules.First(r => r.SelectorText == ".background-check");
            var bgImage = bgCheckRule.Style.GetProperty("background-image");
            ParseFailureDetector.AssertNoParseFailure(bgImage);
            var bgImageList = Assert.IsType<StyleValueList>(bgImage.TypedValue);
            Assert.Equal(4, bgImageList.Count);
            Assert.All(bgImageList, item => Assert.IsType<LinearGradient>(item));

            var overlayRule = sheet.StyleRules.First(r => r.SelectorText == ".background-overlay");
            var overlayBgImage = overlayRule.Style.GetProperty("background-image");
            ParseFailureDetector.AssertNoParseFailure(overlayBgImage);
            var gradient = Assert.IsType<LinearGradient>(overlayBgImage.TypedValue);
            Assert.All(gradient.Stops, s => Assert.True(s.Color.Alpha < 1.0));

            var uiRule = sheet.StyleRules.First(r => r.SelectorText == ".ui-container");
            var uiBgColor = uiRule.Style.GetProperty("background-color");
            ParseFailureDetector.AssertNoParseFailure(uiBgColor);
            var color = Assert.IsType<Color>(uiBgColor.TypedValue);
            Assert.Equal(0.8, color.Alpha, 1);
        }

        [Fact]
        public void CheckerboardPattern_BackgroundImageNoSilentParseFailures()
        {
            var sheet = _parser.Parse(CheckerboardCss);
            var rule = sheet.StyleRules.First();
            var bgImageProp = rule.Style.GetProperty("background-image");

            ParseFailureDetector.AssertNoParseFailure(bgImageProp);
        }

        [Fact]
        public void OverlayGradient_BackgroundImageNoSilentParseFailures()
        {
            var sheet = _parser.Parse(OverlayCss);
            var rule = sheet.StyleRules.First();
            var bgImageProp = rule.Style.GetProperty("background-image");

            ParseFailureDetector.AssertNoParseFailure(bgImageProp);
        }

        [Fact]
        public void CheckerboardPattern_TypedValueIsStyleValueList()
        {
            var sheet = _parser.Parse(CheckerboardCss);
            var rule = sheet.StyleRules.First();

            var bgImageProp = rule.Style.GetProperty("background-image");

            // Use helper to assert proper type (not RawValue fallback)
            var list = ParseFailureDetector.AssertTypedValue<StyleValueList>(bgImageProp);
            Assert.Equal(4, list.Count);

            // Each item should be LinearGradient, not RawValue
            foreach (var item in list)
            {
                Assert.IsType<LinearGradient>(item);
            }
        }

        [Fact]
        public void OverlayGradient_TypedValueIsLinearGradient()
        {
            var sheet = _parser.Parse(OverlayCss);
            var rule = sheet.StyleRules.First();

            var bgImageProp = rule.Style.GetProperty("background-image");

            // Use helper to assert proper type
            var gradient = ParseFailureDetector.AssertTypedValue<LinearGradient>(bgImageProp);

            // Verify gradient has 3 stops with correct alpha values
            var stops = gradient.Stops.ToList();
            Assert.Equal(3, stops.Count);
            Assert.Equal(0.9, stops[0].Color.Alpha, 1);
            Assert.Equal(0.2, stops[1].Color.Alpha, 1);
            Assert.Equal(0.5, stops[2].Color.Alpha, 1);
        }
    }
}
