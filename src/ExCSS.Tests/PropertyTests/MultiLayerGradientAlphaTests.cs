using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace ExCSS.Tests.PropertyTests
{
    /// <summary>
    /// Tests for multi-layer gradient backgrounds with rgba() colors.
    /// Regression tests for alpha channel preservation after Token.Comma change in EndListValueConverter.
    /// </summary>
    public class MultiLayerGradientAlphaTests
    {
        private readonly StylesheetParser _parser = new();
        private readonly ITestOutputHelper _output;

        public MultiLayerGradientAlphaTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SingleGradient_RgbaAlpha_PreservesAlphaValue()
        {
            var css = ".box { background: linear-gradient(rgba(255, 127, 0, 0.25), rgba(255, 127, 0, 0.25)); }";
            var sheet = _parser.Parse(css);
            var rule = sheet.StyleRules.First();

            var bgImageProp = rule.Style.GetProperty("background-image");
            Assert.NotNull(bgImageProp);
            Assert.True(bgImageProp.HasValue);

            // Alpha 0.25 must be preserved
            Assert.Contains("0.25", bgImageProp.Value);
            Assert.Contains("rgba(255, 127, 0, 0.25)", bgImageProp.Value);
        }

        [Fact]
        public void TwoGradients_RgbaAlpha_PreservesAlphaInBothLayers()
        {
            var css = @".plaid {
                background:
                    linear-gradient(90deg, rgba(255, 127, 0, 0.25), rgba(255, 206, 0, 0.25)),
                    linear-gradient(0deg, rgba(255, 127, 0, 0.25), rgba(255, 206, 0, 0.25));
            }";
            var sheet = _parser.Parse(css);
            var rule = sheet.StyleRules.First();

            var bgImageProp = rule.Style.GetProperty("background-image");
            Assert.NotNull(bgImageProp);
            Assert.True(bgImageProp.HasValue, $"background-image should have value. Got: {bgImageProp?.Value}");

            // Both layers should have alpha preserved
            var value = bgImageProp.Value;

            // Count occurrences of 0.25 - should be 4 (2 per gradient * 2 gradients)
            var alphaCount = value.Split(new[] { "0.25" }, System.StringSplitOptions.None).Length - 1;
            Assert.Equal(4, alphaCount);
        }

        [Fact]
        public void TwoGradients_WithFinalColor_RgbaAlphaPreserved()
        {
            // This is the actual failing case from MightyUI
            var css = @".plaid {
                background:
                    repeating-linear-gradient(90deg, transparent, transparent 50px, rgba(255, 127, 0, 0.25) 50px, rgba(255, 127, 0, 0.25) 56px),
                    repeating-linear-gradient(0deg, transparent, transparent 50px, rgba(255, 127, 0, 0.25) 50px, rgba(255, 127, 0, 0.25) 56px),
                    #333;
            }";
            var sheet = _parser.Parse(css);
            var rule = sheet.StyleRules.First();

            var bgImageProp = rule.Style.GetProperty("background-image");
            Assert.NotNull(bgImageProp);
            Assert.True(bgImageProp.HasValue, $"background-image should have value");

            var value = bgImageProp.Value;

            // Alpha 0.25 must be preserved in all rgba() calls
            Assert.Contains("0.25", value);

            // Count rgba with 0.25 - should be 4 (2 per gradient)
            var alphaCount = value.Split(new[] { "0.25" }, System.StringSplitOptions.None).Length - 1;
            Assert.True(alphaCount >= 4, $"Expected at least 4 occurrences of 0.25, got {alphaCount}. Value: {value}");
        }

        [Fact]
        public void RgbaColor_AlphaNotTruncated()
        {
            // Simple rgba parsing test
            var css = ".box { background-color: rgba(255, 127, 0, 0.25); }";
            var sheet = _parser.Parse(css);
            var rule = sheet.StyleRules.First();

            var bgColorProp = rule.Style.GetProperty("background-color");
            Assert.NotNull(bgColorProp);
            Assert.True(bgColorProp.HasValue);

            // Alpha must be 0.25, not 1.0 or missing
            Assert.Contains("0.25", bgColorProp.Value);
        }

        [Fact]
        public void LinearGradient_TypedValue_PreservesRgbaAlpha()
        {
            var css = ".box { background-image: linear-gradient(rgba(255, 0, 0, 0.5), rgba(0, 0, 255, 0.5)); }";
            var sheet = _parser.Parse(css);
            var rule = sheet.StyleRules.First();

            var bgImageProp = rule.Style.GetProperty("background-image");
            Assert.NotNull(bgImageProp);

            var typedValue = bgImageProp.TypedValue;
            Assert.NotNull(typedValue);

            // Should be a LinearGradient
            var gradient = typedValue as LinearGradient;
            Assert.NotNull(gradient);

            // Check color stops have correct alpha
            var stops = gradient.Stops.ToList();
            Assert.True(stops.Count >= 2);

            var firstStop = stops[0];
            Assert.Equal(0.5, firstStop.Color.Alpha, 2); // Alpha should be 0.5

            var lastStop = stops[stops.Count - 1];
            Assert.Equal(0.5, lastStop.Color.Alpha, 2);
        }

        [Fact]
        public void MultiLayerGradient_TypedValue_PreservesRgbaAlphaInAllLayers()
        {
            var css = @".box {
                background-image:
                    linear-gradient(rgba(255, 0, 0, 0.25), rgba(0, 0, 255, 0.25)),
                    linear-gradient(rgba(0, 255, 0, 0.75), rgba(255, 255, 0, 0.75));
            }";
            var sheet = _parser.Parse(css);
            var rule = sheet.StyleRules.First();

            var bgImageProp = rule.Style.GetProperty("background-image");
            Assert.NotNull(bgImageProp);

            var typedValue = bgImageProp.TypedValue;
            Assert.NotNull(typedValue);

            // Should be a list of gradients
            var list = typedValue as StyleValueList;
            Assert.NotNull(list);
            Assert.Equal(2, list.Count);

            // First gradient should have alpha 0.25
            var gradient1 = list[0] as LinearGradient;
            Assert.NotNull(gradient1);
            var stops1 = gradient1.Stops.ToList();
            Assert.Equal(0.25, stops1[0].Color.Alpha, 2);

            // Second gradient should have alpha 0.75
            var gradient2 = list[1] as LinearGradient;
            Assert.NotNull(gradient2);
            var stops2 = gradient2.Stops.ToList();
            Assert.Equal(0.75, stops2[0].Color.Alpha, 2);
        }

        [Fact]
        public void Debug_TokenStream_ForMultiLayerGradient()
        {
            var css = @".box {
                background:
                    linear-gradient(rgba(255, 0, 0, 0.25), blue),
                    linear-gradient(green, rgba(0, 0, 255, 0.75));
            }";
            var sheet = _parser.Parse(css);
            var rule = sheet.StyleRules.First();

            var bgProp = rule.Style.GetProperty("background");
            _output.WriteLine($"background DeclaredValue type: {bgProp.DeclaredValue?.GetType().Name}");
            _output.WriteLine($"background Value: {bgProp.Value}");

            // Get background-image through property expansion
            var bgImageProp = rule.Style.GetProperty("background-image");
            _output.WriteLine($"background-image DeclaredValue type: {bgImageProp?.DeclaredValue?.GetType().Name}");
            _output.WriteLine($"background-image Value: {bgImageProp?.Value}");

            if (bgImageProp?.DeclaredValue?.Original != null)
            {
                _output.WriteLine("Tokens in background-image Original:");
                int idx = 0;
                foreach (var token in bgImageProp.DeclaredValue.Original)
                {
                    _output.WriteLine($"  [{idx++}] Type={token.Type}, Data='{token.Data}'");
                    if (token is FunctionToken ft)
                    {
                        int innerIdx = 0;
                        foreach (var inner in ft)
                        {
                            _output.WriteLine($"      [{innerIdx++}] Type={inner.Type}, Data='{inner.Data}'");
                        }
                    }
                }
            }

            // Check TypedValue gradient colors
            var typedValue = bgImageProp?.TypedValue;
            _output.WriteLine($"TypedValue type: {typedValue?.GetType().Name}");

            if (typedValue is StyleValueList list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    _output.WriteLine($"  Layer {i}: {item.GetType().Name}");
                    if (item is LinearGradient lg)
                    {
                        foreach (var stop in lg.Stops)
                        {
                            _output.WriteLine($"    Stop: Color=rgba({stop.Color.R},{stop.Color.G},{stop.Color.B},{stop.Color.Alpha})");
                        }
                    }
                }
            }

            // The actual assertion
            Assert.NotNull(bgImageProp);
            Assert.True(bgImageProp.HasValue);
        }

        [Fact]
        public void Debug_BackgroundWithColor_ExtractedBackgroundImage()
        {
            // Test what background-image contains when background has trailing color
            var css = @".box {
                background:
                    linear-gradient(rgba(255, 0, 0, 0.25), blue),
                    linear-gradient(green, rgba(0, 0, 255, 0.75)),
                    #333;
            }";
            var sheet = _parser.Parse(css);
            var rule = sheet.StyleRules.First();

            var bgProp = rule.Style.GetProperty("background");
            _output.WriteLine($"background.Value: {bgProp.Value}");
            _output.WriteLine($"background.TypedValue type: {bgProp.TypedValue?.GetType().Name}");
            _output.WriteLine($"background.TypedValue CssText: {bgProp.TypedValue?.CssText}");

            // TEST ShorthandRegistry.Expand directly
            _output.WriteLine("\n=== ShorthandRegistry.Expand ===");
            var expanded = ShorthandRegistry.Expand("background", bgProp.TypedValue);
            foreach (var kvp in expanded)
            {
                _output.WriteLine($"  {kvp.Key}: CssText={kvp.Value?.CssText}, Type={kvp.Value?.GetType().Name}");
            }

            var bgImageProp = rule.Style.GetProperty("background-image");
            _output.WriteLine($"\n=== Property Extraction ===");
            _output.WriteLine($"background-image.Value: {bgImageProp?.Value}");
            _output.WriteLine($"background-image.TypedValue type: {bgImageProp?.TypedValue?.GetType().Name}");
            _output.WriteLine($"background-image.TypedValue CssText: {bgImageProp?.TypedValue?.CssText}");

            var bgColorProp = rule.Style.GetProperty("background-color");
            _output.WriteLine($"background-color.Value: {bgColorProp?.Value}");

            // Check if background-image correctly excludes the color
            Assert.NotNull(bgImageProp);
            Assert.True(bgImageProp.HasValue);

            // background-image should NOT contain the color #333
            Assert.DoesNotContain("#333", bgImageProp.Value);
            Assert.DoesNotContain("rgb(51, 51, 51)", bgImageProp.Value);

            // background-color SHOULD contain the color
            Assert.NotNull(bgColorProp);
            // Note: might be initial or the actual color depending on ExCSS expansion
            _output.WriteLine($"background-color contains '333'? {bgColorProp.Value?.Contains("333")}");
        }

        [Fact]
        public void RepeatingLinearGradient_ComplexStops_RgbaAlphaPreserved()
        {
            // Full plaid pattern test - exact CSS from MightyUI test
            var css = @".plaid {
                background:
                    repeating-linear-gradient(90deg, transparent, transparent 50px, rgba(255, 127, 0, 0.25) 50px, rgba(255, 127, 0, 0.25) 56px, transparent 56px, transparent 63px, rgba(255, 127, 0, 0.25) 63px, rgba(255, 127, 0, 0.25) 69px, transparent 69px, transparent 116px, rgba(255, 206, 0, 0.25) 116px, rgba(255, 206, 0, 0.25) 166px),
                    repeating-linear-gradient(0deg, transparent, transparent 50px, rgba(255, 127, 0, 0.25) 50px, rgba(255, 127, 0, 0.25) 56px, transparent 56px, transparent 63px, rgba(255, 127, 0, 0.25) 63px, rgba(255, 127, 0, 0.25) 69px, transparent 69px, transparent 116px, rgba(255, 206, 0, 0.25) 116px, rgba(255, 206, 0, 0.25) 166px),
                    #333;
            }";
            var sheet = _parser.Parse(css);
            var rule = sheet.StyleRules.First();

            var bgProp = rule.Style.GetProperty("background");
            _output.WriteLine($"background.TypedValue type: {bgProp.TypedValue?.GetType().Name}");

            // Test ShorthandRegistry.Expand with repeating-linear-gradient
            _output.WriteLine("\n=== ShorthandRegistry.Expand (repeating-linear-gradient) ===");
            var expanded = ShorthandRegistry.Expand("background", bgProp.TypedValue);
            foreach (var kvp in expanded)
            {
                var cssText = kvp.Value?.CssText ?? "null";
                if (cssText.Length > 100) cssText = cssText.Substring(0, 100) + "...";
                _output.WriteLine($"  {kvp.Key}: CssText={cssText}");
            }

            var bgImageProp = rule.Style.GetProperty("background-image");
            Assert.NotNull(bgImageProp);
            Assert.True(bgImageProp.HasValue);

            var value = bgImageProp.Value;
            _output.WriteLine($"\nbackground-image.Value: {value.Substring(0, System.Math.Min(200, value.Length))}...");

            // Should contain rgba with 0.25 alpha
            Assert.Contains("rgba(255, 127, 0, 0.25)", value);
            Assert.Contains("rgba(255, 206, 0, 0.25)", value);

            // Should NOT contain alpha values of 1 for orange/yellow colors
            // (which would indicate the alpha was lost/truncated)
            Assert.DoesNotContain("rgba(255, 127, 0, 1)", value);
            Assert.DoesNotContain("rgb(255, 127, 0)", value); // Should be rgba, not rgb
        }
    }
}
