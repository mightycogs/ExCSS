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

        [Fact]
        public void Parse_CalcSubtraction_HasTypedAST()
        {
            var sheet = ParseFixture("Values", "022_calc_subtraction.css");
            var rule = GetSingleStyleRule(sheet);
            var widthProp = rule.Style.GetProperty("width");

            Assert.NotNull(widthProp);
            var calcValue = widthProp.TypedValue as CalcValue;
            Assert.NotNull(calcValue);
            Assert.NotNull(calcValue.Root);

            var binary = Assert.IsType<CalcBinaryExpression>(calcValue.Root);
            Assert.Equal(CalcOperator.Subtract, binary.Operator);

            var left = Assert.IsType<CalcLiteralExpression>(binary.Left);
            var leftPercent = Assert.IsType<Percent>(left.Value);
            Assert.Equal(100d, leftPercent.Value);

            var right = Assert.IsType<CalcLiteralExpression>(binary.Right);
            var rightLength = Assert.IsType<Length>(right.Value);
            Assert.Equal(20d, rightLength.Value);
            Assert.Equal(Length.Unit.Px, rightLength.Type);
        }

        [Fact]
        public void Parse_CalcMultiplication_HasTypedAST()
        {
            var sheet = ParseFixture("Values", "023_calc_multiplication.css");
            var rule = GetSingleStyleRule(sheet);
            var widthProp = rule.Style.GetProperty("width");

            Assert.NotNull(widthProp);
            var calcValue = widthProp.TypedValue as CalcValue;
            Assert.NotNull(calcValue);
            Assert.NotNull(calcValue.Root);

            var binary = Assert.IsType<CalcBinaryExpression>(calcValue.Root);
            Assert.Equal(CalcOperator.Multiply, binary.Operator);

            var left = Assert.IsType<CalcLiteralExpression>(binary.Left);
            var leftPercent = Assert.IsType<Percent>(left.Value);
            Assert.Equal(50d, leftPercent.Value);

            var right = Assert.IsType<CalcLiteralExpression>(binary.Right);
            var rightNumber = Assert.IsType<Number>(right.Value);
            Assert.Equal(2d, rightNumber.Value);
        }

        [Fact]
        public void Parse_CalcWithVar_HasTypedAST()
        {
            var sheet = ParseFixture("Values", "024_calc_with_var_subtract.css");
            var rule = GetSingleStyleRule(sheet);
            var marginProp = rule.Style.GetProperty("margin");

            Assert.NotNull(marginProp);
            var calcValue = marginProp.TypedValue as CalcValue;
            Assert.NotNull(calcValue);
            Assert.NotNull(calcValue.Root);

            var binary = Assert.IsType<CalcBinaryExpression>(calcValue.Root);
            Assert.Equal(CalcOperator.Subtract, binary.Operator);

            var left = Assert.IsType<CalcLiteralExpression>(binary.Left);
            Assert.IsType<Percent>(left.Value);

            var right = Assert.IsType<CalcVarExpression>(binary.Right);
            Assert.Equal("--spacing", right.Variable.VariableName);
        }

        [Fact]
        public void Parse_CalcDivision_HasTypedAST()
        {
            var sheet = Parser.Parse(".element { width: calc(100% / 2); }");
            var rule = GetSingleStyleRule(sheet);
            var widthProp = rule.Style.GetProperty("width");

            Assert.NotNull(widthProp);
            var calcValue = widthProp.TypedValue as CalcValue;
            Assert.NotNull(calcValue);
            Assert.NotNull(calcValue.Root);

            var divideExpr = Assert.IsType<CalcBinaryExpression>(calcValue.Root);
            Assert.Equal(CalcOperator.Divide, divideExpr.Operator);

            var left = Assert.IsType<CalcLiteralExpression>(divideExpr.Left);
            Assert.IsType<Percent>(left.Value);

            var right = Assert.IsType<CalcLiteralExpression>(divideExpr.Right);
            var rightNumber = Assert.IsType<Number>(right.Value);
            Assert.Equal(2d, rightNumber.Value);
        }

        [Fact]
        public void Parse_CalcSimpleLength_HasTypedAST()
        {
            var sheet = ParseFixture("Values", "026_calc_simple_length.css");
            var rule = GetSingleStyleRule(sheet);
            var widthProp = rule.Style.GetProperty("width");

            Assert.NotNull(widthProp);
            var calcValue = widthProp.TypedValue as CalcValue;
            Assert.NotNull(calcValue);
            Assert.NotNull(calcValue.Root);

            var literal = Assert.IsType<CalcLiteralExpression>(calcValue.Root);
            var length = Assert.IsType<Length>(literal.Value);
            Assert.Equal(10d, length.Value);
            Assert.Equal(Length.Unit.Px, length.Type);
        }

        [Fact]
        public void Shadow_TryParse_FromStyleValueList()
        {
            var sheet = Parser.Parse(".box { box-shadow: 0 4px 16px rgba(0,0,0,0.15); }");
            var rule = GetSingleStyleRule(sheet);
            var shadowProp = rule.Style.GetProperty("box-shadow");

            Assert.NotNull(shadowProp);
            var typedValue = shadowProp.TypedValue;
            Assert.NotNull(typedValue);

            var shadow = Shadow.TryParse(typedValue);
            Assert.NotNull(shadow);
            Assert.Equal(0f, shadow.OffsetX.Value);
            Assert.Equal(4f, shadow.OffsetY.Value);
            Assert.Equal(16f, shadow.BlurRadius.Value);
            Assert.Equal(0f, shadow.SpreadRadius.Value);
            Assert.False(shadow.IsInset);
        }

        [Fact]
        public void Shadow_TryParse_WithInset()
        {
            var sheet = Parser.Parse(".box { box-shadow: inset 2px 3px 4px 5px red; }");
            var rule = GetSingleStyleRule(sheet);
            var shadowProp = rule.Style.GetProperty("box-shadow");

            // Pass cssText to detect 'inset' keyword
            var shadow = Shadow.TryParse(shadowProp.TypedValue, shadowProp.Value);
            Assert.NotNull(shadow);
            Assert.True(shadow.IsInset);
            Assert.Equal(2f, shadow.OffsetX.Value);
            Assert.Equal(3f, shadow.OffsetY.Value);
            Assert.Equal(4f, shadow.BlurRadius.Value);
            Assert.Equal(5f, shadow.SpreadRadius.Value);
            Assert.Equal(255, shadow.Color.R);
            Assert.Equal(0, shadow.Color.G);
            Assert.Equal(0, shadow.Color.B);
        }

        [Fact]
        public void Shadow_TryParse_MinimalTwoLengths()
        {
            var sheet = Parser.Parse(".box { box-shadow: 5px 10px; }");
            var rule = GetSingleStyleRule(sheet);
            var shadowProp = rule.Style.GetProperty("box-shadow");

            var shadow = Shadow.TryParse(shadowProp.TypedValue);
            Assert.NotNull(shadow);
            Assert.Equal(5f, shadow.OffsetX.Value);
            Assert.Equal(10f, shadow.OffsetY.Value);
            Assert.Equal(0f, shadow.BlurRadius.Value);
            Assert.Equal(0f, shadow.SpreadRadius.Value);
        }

        [Fact]
        public void Shadow_TryParse_ReturnsNullForInvalidInput()
        {
            Assert.Null(Shadow.TryParse((IStyleValue)null));
            Assert.Null(Shadow.TryParse(new StyleValueList(new Length(5f, Length.Unit.Px))));
        }

        [Fact]
        public void LinearGradient_TypedValue_ReturnsLinearGradient()
        {
            var sheet = Parser.Parse(".box { background: linear-gradient(45deg, red 0%, blue 100%); }");
            var rule = GetSingleStyleRule(sheet);
            var bgProp = rule.Style.GetProperty("background");

            Assert.NotNull(bgProp);
            var typedValue = bgProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsType<LinearGradient>(typedValue);

            var gradient = (LinearGradient)typedValue;
            Assert.False(gradient.IsRepeating);
            Assert.Equal(45f, gradient.Angle.Value);
            Assert.Equal(Angle.Unit.Deg, gradient.Angle.Type);

            var stops = gradient.Stops.ToArray();
            Assert.Equal(2, stops.Length);
            Assert.Equal(255, stops[0].Color.R);
            Assert.Equal(0, stops[0].Color.G);
            Assert.Equal(0f, stops[0].Location.Value);
            Assert.Equal(0, stops[1].Color.R);
            Assert.Equal(0, stops[1].Color.G);
            Assert.Equal(255, stops[1].Color.B);
            Assert.Equal(100f, stops[1].Location.Value);
        }

        [Fact]
        public void RepeatingLinearGradient_TypedValue_ReturnsLinearGradientWithRepeating()
        {
            var sheet = Parser.Parse(".box { background: repeating-linear-gradient(90deg, #000, #fff 10px); }");
            var rule = GetSingleStyleRule(sheet);
            var bgProp = rule.Style.GetProperty("background");

            Assert.NotNull(bgProp);
            var typedValue = bgProp.TypedValue;
            Assert.IsType<LinearGradient>(typedValue);

            var gradient = (LinearGradient)typedValue;
            Assert.True(gradient.IsRepeating);
            Assert.Equal(90f, gradient.Angle.Value);
        }

        [Fact]
        public void RadialGradient_TypedValue_ReturnsRadialGradient()
        {
            var sheet = Parser.Parse(".box { background: radial-gradient(red, blue); }");
            var rule = GetSingleStyleRule(sheet);
            var bgProp = rule.Style.GetProperty("background");

            Assert.NotNull(bgProp);
            var typedValue = bgProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsType<RadialGradient>(typedValue);

            var gradient = (RadialGradient)typedValue;
            Assert.False(gradient.IsRepeating);

            var stops = gradient.Stops.ToArray();
            Assert.Equal(2, stops.Length);
        }

        [Fact]
        public void LinearGradient_CssText_RoundTrip()
        {
            var sheet = Parser.Parse(".box { background: linear-gradient(180deg, red 0%, blue 100%); }");
            var rule = GetSingleStyleRule(sheet);
            var bgProp = rule.Style.GetProperty("background");
            var gradient = bgProp.TypedValue as LinearGradient;

            Assert.NotNull(gradient);
            var cssText = gradient.CssText;
            Assert.Contains("linear-gradient(", cssText);
            Assert.Contains("rgb(255, 0, 0)", cssText);
            Assert.Contains("rgb(0, 0, 255)", cssText);
        }

        [Fact]
        public void GradientStop_ImplementsIStyleValue()
        {
            var stop = new GradientStop(Color.Red, new Length(50f, Length.Unit.Percent));

            Assert.IsAssignableFrom<IStyleValue>(stop);
            Assert.Equal(StyleValueType.Gradient, stop.Type);
            Assert.Contains("rgb(255, 0, 0)", stop.CssText);
            Assert.Contains("50%", stop.CssText);
        }
    }
}
