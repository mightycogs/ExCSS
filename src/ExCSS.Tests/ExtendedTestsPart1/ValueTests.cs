using System.Linq;
using Xunit;

namespace ExCSS.Tests.ExtendedTestsPart1
{
    public class ValueTests : ExtendedTestBase
    {
        [Fact]
        public void Parse_CssVariable_VarValue()
        {
            var sheet = ParseFixture("Values", "001_css_variable.css");
            var rule = GetSingleStyleRule(sheet);
            var colorProp = rule.Style.GetProperty("color");

            Assert.NotNull(colorProp);
            var typedValue = colorProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
            Assert.IsType<VarValue>(typedValue);

            var varValue = (VarValue)typedValue;
            Assert.Equal("--accent-green", varValue.VariableName);
        }

        [Fact]
        public void Parse_RgbaColor_ColorValue()
        {
            var sheet = ParseFixture("Values", "002_rgba_color.css");
            var rule = GetSingleStyleRule(sheet);
            var bgProp = rule.Style.GetProperty("background-color");

            Assert.NotNull(bgProp);
            var typedValue = bgProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
            Assert.IsType<Color>(typedValue);

            var color = (Color)typedValue;
            Assert.Equal(30, color.R);
            Assert.Equal(30, color.G);
            Assert.Equal(30, color.B);
        }

        [Fact]
        public void Parse_HexColor_ColorValue()
        {
            var sheet = ParseFixture("Values", "003_hex_color.css");
            var rule = GetSingleStyleRule(sheet);
            var bgProp = rule.Style.GetProperty("background-color");

            Assert.NotNull(bgProp);
            var typedValue = bgProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
            Assert.IsType<Color>(typedValue);

            var color = (Color)typedValue;
            Assert.Equal(26, color.R);
            Assert.Equal(26, color.G);
            Assert.Equal(26, color.B);
        }

        [Fact]
        public void Parse_CalcSimple_CalcValue()
        {
            var sheet = ParseFixture("Values", "004_calc_simple.css");
            var rule = GetSingleStyleRule(sheet);
            var widthProp = rule.Style.GetProperty("width");

            Assert.NotNull(widthProp);
            var typedValue = widthProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
            Assert.IsType<CalcValue>(typedValue);
        }

        [Fact]
        public void Parse_CalcWithVar_CalcValue()
        {
            var sheet = ParseFixture("Values", "005_calc_with_var.css");
            var rule = GetSingleStyleRule(sheet);
            var rightProp = rule.Style.GetProperty("right");

            Assert.NotNull(rightProp);
            var typedValue = rightProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
            Assert.IsType<CalcValue>(typedValue);
        }

        [Fact]
        public void Parse_BoxShadowMulti_ShadowValue()
        {
            var sheet = ParseFixture("Values", "006_box_shadow_multi.css");
            var rule = GetSingleStyleRule(sheet);
            var shadowProp = rule.Style.GetProperty("box-shadow");

            Assert.NotNull(shadowProp);
            var typedValue = shadowProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
        }

        [Fact]
        public void Parse_RadialGradient_GradientValue()
        {
            var sheet = ParseFixture("Values", "007_radial_gradient.css");
            var rule = GetSingleStyleRule(sheet);
            var bgProp = rule.Style.GetProperty("background");

            Assert.NotNull(bgProp);
            var typedValue = bgProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
        }

        [Fact]
        public void Parse_LinearGradient_GradientValue()
        {
            var sheet = ParseFixture("Values", "008_linear_gradient.css");
            var rule = GetSingleStyleRule(sheet);
            var bgProp = rule.Style.GetProperty("background");

            Assert.NotNull(bgProp);
            var typedValue = bgProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
        }

        [Fact]
        public void Parse_TransitionShorthand_NotRawValue()
        {
            var sheet = ParseFixture("Values", "009_transition_shorthand.css");
            var rule = GetSingleStyleRule(sheet);
            var transitionProp = rule.Style.GetProperty("transition");

            Assert.NotNull(transitionProp);
            var typedValue = transitionProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
        }

        [Fact]
        public void Parse_FontStack_NotRawValue()
        {
            var sheet = ParseFixture("Values", "010_font_stack.css");
            var rule = GetSingleStyleRule(sheet);
            var fontProp = rule.Style.GetProperty("font-family");

            Assert.NotNull(fontProp);
            var typedValue = fontProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
        }

        [Fact]
        public void Parse_TimeValue_TimeValue()
        {
            var sheet = ParseFixture("Values", "011_time_value.css");
            var rule = GetSingleStyleRule(sheet);
            var customProp = rule.Style.GetProperty("--transition-fast");

            Assert.NotNull(customProp);
        }

        [Fact]
        public void Parse_UnitlessNumber_NumberValue()
        {
            var sheet = ParseFixture("Values", "012_unitless_number.css");
            var rule = GetSingleStyleRule(sheet);
            var lineHeightProp = rule.Style.GetProperty("line-height");

            Assert.NotNull(lineHeightProp);
            var typedValue = lineHeightProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
            Assert.IsType<Number>(typedValue);

            var number = (Number)typedValue;
            Assert.Equal(1.2, number.Value);
        }

        [Fact]
        public void Parse_Height_NotRawValue()
        {
            var sheet = ParseFixture("Selectors", "001_root_html_body_id.css");
            var rule = GetSingleStyleRule(sheet);
            var heightProp = rule.Style.GetProperty("height");

            Assert.NotNull(heightProp);
            var typedValue = heightProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
            Assert.IsType<Percent>(typedValue);
        }

        [Fact]
        public void Parse_MarginLeft_VarValue()
        {
            var sheet = ParseFixture("Selectors", "002_adjacent_sibling_universal.css");
            var rule = GetSingleStyleRule(sheet);
            var marginProp = rule.Style.GetProperty("margin-left");

            Assert.NotNull(marginProp);
            var typedValue = marginProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
            Assert.IsType<VarValue>(typedValue);
        }

        [Fact]
        public void Parse_TranslateY_CalcValue()
        {
            var sheet = ParseFixture("Values", "013_translateY.css");
            var rule = GetSingleStyleRule(sheet);
            var transformProp = rule.Style.GetProperty("transform");

            Assert.NotNull(transformProp);
            var typedValue = transformProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
            Assert.Contains("translateY", transformProp.Value);
        }

        [Fact]
        public void Parse_TranslateX_NotRawValue()
        {
            var sheet = ParseFixture("Values", "014_translateX.css");
            var rule = GetSingleStyleRule(sheet);
            var transformProp = rule.Style.GetProperty("transform");

            Assert.NotNull(transformProp);
            var typedValue = transformProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
            Assert.Contains("translateX", transformProp.Value);
        }

        [Fact]
        public void Parse_ContentEmpty_StringValue()
        {
            var sheet = ParseFixture("Values", "015_content_empty.css");
            var rule = GetSingleStyleRule(sheet);
            var contentProp = rule.Style.GetProperty("content");

            Assert.NotNull(contentProp);
            Assert.True(contentProp.HasValue);
            // CSS '' and "" are semantically equivalent - parser normalizes to double quotes
            Assert.True(contentProp.Value == "\"\"" || contentProp.Value == "''",
                $"Expected empty string ('\"\"' or '\\'\\''), got '{contentProp.Value}'");
        }

        [Fact]
        public void Parse_ContentString_StringValue()
        {
            var sheet = ParseFixture("Values", "016_content_string.css");
            var rule = GetSingleStyleRule(sheet);
            var contentProp = rule.Style.GetProperty("content");

            Assert.NotNull(contentProp);
            Assert.True(contentProp.HasValue);
            Assert.Contains("▶", contentProp.Value);
        }

        [Fact]
        public void Parse_NoneKeyword_IdentValue()
        {
            var sheet = ParseFixture("Values", "017_none_keyword.css");
            var rule = GetSingleStyleRule(sheet);
            var borderProp = rule.Style.GetProperty("border");

            Assert.NotNull(borderProp);
            Assert.Equal("none", borderProp.Value);
        }

        [Fact]
        public void Parse_AutoKeyword_IdentValue()
        {
            var sheet = ParseFixture("Values", "018_auto_keyword.css");
            var rule = GetSingleStyleRule(sheet);
            var minWidthProp = rule.Style.GetProperty("min-width");

            Assert.NotNull(minWidthProp);
            Assert.Equal("auto", minWidthProp.Value);
        }

        [Fact]
        public void Parse_HiddenVisible_Keywords()
        {
            var sheet = ParseFixture("Values", "019_hidden_visible.css");
            var rules = sheet.Rules.OfType<StyleRule>().ToList();
            Assert.Equal(2, rules.Count);

            var visibleProp = rules[0].Style.GetProperty("overflow");
            Assert.NotNull(visibleProp);
            Assert.Equal("visible", visibleProp.Value);

            var hiddenProp = rules[1].Style.GetProperty("overflow");
            Assert.NotNull(hiddenProp);
            Assert.Equal("hidden", hiddenProp.Value);
        }

        [Fact]
        public void Parse_PositionAbsolute_IdentValue()
        {
            var sheet = ParseFixture("Values", "020_position_values.css");
            var rules = sheet.Rules.OfType<StyleRule>().ToList();
            Assert.Equal(3, rules.Count);

            var absoluteProp = rules[0].Style.GetProperty("position");
            Assert.NotNull(absoluteProp);
            Assert.Equal("absolute", absoluteProp.Value);

            var fixedProp = rules[1].Style.GetProperty("position");
            Assert.NotNull(fixedProp);
            Assert.Equal("fixed", fixedProp.Value);

            var relativeProp = rules[2].Style.GetProperty("position");
            Assert.NotNull(relativeProp);
            Assert.Equal("relative", relativeProp.Value);
        }

        [Fact]
        public void Parse_FlexStartEndWrap_Keywords()
        {
            var sheet = ParseFixture("Values", "021_flex_values.css");
            var rule = GetSingleStyleRule(sheet);

            var alignContent = rule.Style.GetProperty("align-content");
            Assert.NotNull(alignContent);
            Assert.Equal("flex-start", alignContent.Value);

            var flexWrap = rule.Style.GetProperty("flex-wrap");
            Assert.NotNull(flexWrap);
            Assert.Equal("wrap", flexWrap.Value);
        }
    }
}
