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
    }
}
