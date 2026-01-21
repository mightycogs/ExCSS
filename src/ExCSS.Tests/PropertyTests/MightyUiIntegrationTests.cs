using System.Linq;
using Xunit;

namespace ExCSS.Tests.PropertyTests
{
    public class MightyUiIntegrationTests
    {
        private readonly StylesheetParser _parser = new();

        [Fact]
        public void BorderRadius_SimpleValue_ParsesAndExpands()
        {
            var sheet = _parser.Parse(".item-slot { border-radius: 4px }");
            var rule = sheet.StyleRules.First();

            var prop = rule.Style.GetProperty("border-radius");
            Assert.NotNull(prop);
            Assert.Equal("4px", prop.Value);

            var typedValue = prop.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsType<RawValue>(typedValue);
            Assert.Equal("4px", typedValue.ToString());

            var expanded = ShorthandRegistry.Expand("border-radius", typedValue);
            Assert.Equal(4, expanded.Count);
            Assert.Contains("border-top-left-radius", expanded.Keys);
            Assert.Contains("border-top-right-radius", expanded.Keys);
            Assert.Contains("border-bottom-right-radius", expanded.Keys);
            Assert.Contains("border-bottom-left-radius", expanded.Keys);

            foreach (var kvp in expanded)
            {
                Assert.IsType<RawValue>(kvp.Value);
                Assert.Equal("4px", kvp.Value.ToString());
            }
        }

        [Fact]
        public void BoxShadow_WithRgba_ParsesCorrectly()
        {
            var sheet = _parser.Parse(".notification-badge { box-shadow: 0 2px 4px rgba(0,0,0,0.5) }");
            var rule = sheet.StyleRules.First();

            var prop = rule.Style.GetProperty("box-shadow");
            Assert.NotNull(prop);
            Assert.Equal("0 2px 4px rgba(0, 0, 0, 0.5)", prop.Value);

            var typedValue = prop.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);

            var shadow = Shadow.TryParse(typedValue);
            Assert.NotNull(shadow);
            Assert.Equal(0f, shadow.OffsetX.Value);
            Assert.Equal(2f, shadow.OffsetY.Value);
            Assert.Equal(4f, shadow.BlurRadius.Value);
            Assert.Equal(0f, shadow.SpreadRadius.Value);
            Assert.False(shadow.IsInset);

            Assert.Equal(0, shadow.Color.R);
            Assert.Equal(0, shadow.Color.G);
            Assert.Equal(0, shadow.Color.B);
            Assert.Equal(128, shadow.Color.A); // 0.5 * 255 ≈ 128
        }

        [Fact]
        public void BorderRadius_Elliptical_ParsesHorizontalVertical()
        {
            var sheet = _parser.Parse(".corner { border-top-left-radius: 2px 4px }");
            var rule = sheet.StyleRules.First();

            var prop = rule.Style.GetProperty("border-top-left-radius");
            Assert.NotNull(prop);
            Assert.Equal("2px 4px", prop.Value);

            var typedValue = prop.TypedValue;
            Assert.NotNull(typedValue);

            Assert.IsType<StyleValueList>(typedValue);
            var list = (StyleValueList)typedValue;
            Assert.Equal(2, list.Count);

            var rx = Assert.IsType<Length>(list[0]);
            var ry = Assert.IsType<Length>(list[1]);
            Assert.Equal(2d, rx.Value);
            Assert.Equal(4d, ry.Value);
        }

        [Fact]
        public void BorderRadius_Percentage_ParsesCorrectly()
        {
            var sheet = _parser.Parse(".circle { border-radius: 50% }");
            var rule = sheet.StyleRules.First();

            var prop = rule.Style.GetProperty("border-radius");
            Assert.NotNull(prop);
            Assert.Equal("50%", prop.Value);

            var typedValue = prop.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsType<RawValue>(typedValue);
            Assert.Equal("50%", typedValue.ToString());

            var expanded = ShorthandRegistry.Expand("border-radius", typedValue);
            Assert.Equal(4, expanded.Count);
            foreach (var kvp in expanded)
            {
                Assert.Equal("50%", kvp.Value.ToString());
            }
        }

        [Fact]
        public void BoxShadow_None_ParsesAsKeyword()
        {
            var sheet = _parser.Parse(".flat { box-shadow: none }");
            var rule = sheet.StyleRules.First();

            var prop = rule.Style.GetProperty("box-shadow");
            Assert.NotNull(prop);
            Assert.Equal("none", prop.Value);

            var typedValue = prop.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsType<KeywordValue>(typedValue);
        }
    }
}
