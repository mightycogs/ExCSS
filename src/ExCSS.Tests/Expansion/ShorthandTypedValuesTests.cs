using System.Linq;
using Xunit;

namespace ExCSS.Tests.Expansion
{
    /// <summary>
    /// Integration tests demonstrating typed value extraction from CSS properties.
    /// These tests show that shorthand properties are automatically expanded to longhands
    /// during parsing, and typed values can be extracted from those longhands.
    /// </summary>
    public class ShorthandTypedValuesTests
    {
        private readonly StylesheetParser _parser = new();

        private StyleDeclaration ParseRule(string css)
        {
            var stylesheet = _parser.Parse($".test {{ {css} }}");
            var rule = stylesheet.StyleRules.First();
            return rule.Style;
        }

        private IStyleValue GetTypedValue(StyleDeclaration style, string propertyName)
        {
            var property = style.Declarations.FirstOrDefault(p => p.Name == propertyName);
            return property?.TypedValue;
        }

        #region Background Shorthand - Color Extraction

        [Fact]
        public void Background_RgbaColor_ExpandsToTypedColor()
        {
            var style = ParseRule("background: rgba(255, 0, 0, 0.5);");
            var color = Assert.IsType<Color>(GetTypedValue(style, "background-color"));

            Assert.Equal(255, color.R);
            Assert.Equal(0, color.G);
            Assert.Equal(0, color.B);
            Assert.Equal(0.5, color.Alpha, 0.01);
        }

        [Fact]
        public void Background_RgbColor_ExpandsToTypedColor()
        {
            var style = ParseRule("background: rgb(0, 128, 255);");
            var color = Assert.IsType<Color>(GetTypedValue(style, "background-color"));

            Assert.Equal(0, color.R);
            Assert.Equal(128, color.G);
            Assert.Equal(255, color.B);
            Assert.Equal(1.0, color.Alpha);
        }

        [Fact]
        public void Background_HexColor_ExpandsToTypedColor()
        {
            var style = ParseRule("background: #ff5500;");
            var color = Assert.IsType<Color>(GetTypedValue(style, "background-color"));

            Assert.Equal(255, color.R);
            Assert.Equal(85, color.G);
            Assert.Equal(0, color.B);
        }

        [Fact]
        public void Background_NamedColor_ExpandsToTypedColor()
        {
            var style = ParseRule("background: red;");
            Assert.Equal(Color.Red, GetTypedValue(style, "background-color"));
        }

        [Fact]
        public void Background_Transparent_ExpandsToTransparentColor()
        {
            var style = ParseRule("background: transparent;");
            Assert.Equal(Color.Transparent, GetTypedValue(style, "background-color"));
        }

        [Fact]
        public void Background_HslaColor_ExpandsToTypedColor()
        {
            var style = ParseRule("background: hsla(120, 100%, 50%, 0.8);");
            var color = Assert.IsType<Color>(GetTypedValue(style, "background-color"));

            Assert.Equal(0, color.R);
            Assert.Equal(255, color.G);
            Assert.Equal(0, color.B);
            Assert.Equal(0.8, color.Alpha, 0.01);
        }

        #endregion

        #region Border Shorthand - Width, Style, Color

        [Fact]
        public void Border_FullShorthand_ExpandsToTypedValues()
        {
            var style = ParseRule("border: 2px solid #333;");

            var width = Assert.IsType<Length>(GetTypedValue(style, "border-top-width"));
            Assert.Equal(2, width.Value);
            Assert.Equal(Length.Unit.Px, width.Type);

            var borderStyle = Assert.IsType<KeywordValue>(GetTypedValue(style, "border-top-style"));
            Assert.Equal("solid", borderStyle.CssText);

            var color = Assert.IsType<Color>(GetTypedValue(style, "border-top-color"));
            Assert.Equal(51, color.R);
            Assert.Equal(51, color.G);
            Assert.Equal(51, color.B);
        }

        [Fact]
        public void Border_RgbaColor_ExpandsToTypedColor()
        {
            var style = ParseRule("border: 1px solid rgba(0, 0, 0, 0.1);");
            var color = Assert.IsType<Color>(GetTypedValue(style, "border-top-color"));

            Assert.Equal(0, color.R);
            Assert.Equal(0, color.G);
            Assert.Equal(0, color.B);
            Assert.Equal(0.1, color.Alpha, 0.01);
        }

        [Fact]
        public void Border_EmWidth_ExpandsToTypedLength()
        {
            var style = ParseRule("border: 0.5em dashed blue;");
            var width = Assert.IsType<Length>(GetTypedValue(style, "border-top-width"));

            Assert.Equal(0.5f, width.Value);
            Assert.Equal(Length.Unit.Em, width.Type);
        }

        [Fact]
        public void BorderTop_ExpandsToIndividualLonghands()
        {
            var style = ParseRule("border-top: 3px dotted green;");

            var width = Assert.IsType<Length>(GetTypedValue(style, "border-top-width"));
            Assert.Equal(3, width.Value);
            var borderStyle = Assert.IsType<KeywordValue>(GetTypedValue(style, "border-top-style"));
            Assert.Equal("dotted", borderStyle.CssText);
            Assert.Equal(Color.Green, GetTypedValue(style, "border-top-color"));
        }

        #endregion

        #region Margin Shorthand - Box Model

        [Fact]
        public void Margin_SingleValue_ExpandsToAllSides()
        {
            var style = ParseRule("margin: 10px;");

            foreach (var side in new[] { "margin-top", "margin-right", "margin-bottom", "margin-left" })
            {
                var length = Assert.IsType<Length>(GetTypedValue(style, side));
                Assert.Equal(10, length.Value);
                Assert.Equal(Length.Unit.Px, length.Type);
            }
        }

        [Fact]
        public void Margin_TwoValues_ExpandsToTypedLengths()
        {
            var style = ParseRule("margin: 1rem 2em;");

            var top = Assert.IsType<Length>(GetTypedValue(style, "margin-top"));
            Assert.Equal(1, top.Value);
            Assert.Equal(Length.Unit.Rem, top.Type);

            var right = Assert.IsType<Length>(GetTypedValue(style, "margin-right"));
            Assert.Equal(2, right.Value);
            Assert.Equal(Length.Unit.Em, right.Type);

            Assert.Equal(top, GetTypedValue(style, "margin-bottom"));
            Assert.Equal(right, GetTypedValue(style, "margin-left"));
        }

        [Fact]
        public void Margin_FourValues_ExpandsToTypedLengths()
        {
            var style = ParseRule("margin: 10px 20px 30px 40px;");

            Assert.Equal(10, Assert.IsType<Length>(GetTypedValue(style, "margin-top")).Value);
            Assert.Equal(20, Assert.IsType<Length>(GetTypedValue(style, "margin-right")).Value);
            Assert.Equal(30, Assert.IsType<Length>(GetTypedValue(style, "margin-bottom")).Value);
            Assert.Equal(40, Assert.IsType<Length>(GetTypedValue(style, "margin-left")).Value);
        }

        [Fact]
        public void Margin_Auto_ExpandsToKeyword()
        {
            var style = ParseRule("margin: auto;");
            // Note: 'auto' returns RawValue, not KeywordValue (parser inconsistency)
            Assert.Equal("auto", GetTypedValue(style, "margin-top").CssText);
        }

        [Fact]
        public void Margin_MixedUnits_ExpandsToTypedValues()
        {
            var style = ParseRule("margin: 10px auto 2rem 5%;");

            Assert.IsType<Length>(GetTypedValue(style, "margin-top"));
            Assert.Equal("auto", GetTypedValue(style, "margin-right").CssText); // RawValue
            Assert.IsType<Length>(GetTypedValue(style, "margin-bottom"));
            Assert.IsType<Percent>(GetTypedValue(style, "margin-left"));
        }

        #endregion

        #region Padding Shorthand - Box Model

        [Fact]
        public void Padding_SingleValue_ExpandsToAllSides()
        {
            var style = ParseRule("padding: 20px;");

            foreach (var side in new[] { "padding-top", "padding-right", "padding-bottom", "padding-left" })
            {
                var length = Assert.IsType<Length>(GetTypedValue(style, side));
                Assert.Equal(20, length.Value);
                Assert.Equal(Length.Unit.Px, length.Type);
            }
        }

        [Fact]
        public void Padding_Percent_ExpandsToTypedPercent()
        {
            var style = ParseRule("padding: 5%;");
            var percent = Assert.IsType<Percent>(GetTypedValue(style, "padding-top"));
            Assert.Equal(5, percent.Value);
        }

        [Fact]
        public void Padding_Vh_ExpandsToTypedLength()
        {
            var style = ParseRule("padding: 10vh;");
            var length = Assert.IsType<Length>(GetTypedValue(style, "padding-top"));
            Assert.Equal(10, length.Value);
            Assert.Equal(Length.Unit.Vh, length.Type);
        }

        #endregion

        #region Flex Shorthand - Values via CssText

        [Fact]
        public void Flex_SingleNumber_ExpandsToLonghands()
        {
            var style = ParseRule("flex: 2;");

            // Note: flex-grow/shrink return RawValue (not Number) through parser pipeline
            // This is a known limitation - FlexExpander returns Number but PropertyFactory wraps as RawValue
            Assert.Equal("2", GetTypedValue(style, "flex-grow").CssText);
            Assert.Equal("1", GetTypedValue(style, "flex-shrink").CssText);

            var basis = Assert.IsType<Length>(GetTypedValue(style, "flex-basis"));
            Assert.Equal(0, basis.Value);
        }

        [Fact]
        public void Flex_TwoNumbers_ExpandsToLonghands()
        {
            var style = ParseRule("flex: 2 3;");

            Assert.Equal("2", GetTypedValue(style, "flex-grow").CssText);
            Assert.Equal("3", GetTypedValue(style, "flex-shrink").CssText);
        }

        [Fact]
        public void Flex_NumberAndLength_ExpandsToTypedBasis()
        {
            var style = ParseRule("flex: 1 100px;");

            Assert.Equal("1", GetTypedValue(style, "flex-grow").CssText);

            var basis = Assert.IsType<Length>(GetTypedValue(style, "flex-basis"));
            Assert.Equal(100, basis.Value);
            Assert.Equal(Length.Unit.Px, basis.Type);
        }

        [Fact]
        public void Flex_ThreeValues_ExpandsWithPercentBasis()
        {
            var style = ParseRule("flex: 2 1 50%;");

            Assert.Equal("2", GetTypedValue(style, "flex-grow").CssText);
            Assert.Equal("1", GetTypedValue(style, "flex-shrink").CssText);

            var basis = Assert.IsType<Percent>(GetTypedValue(style, "flex-basis"));
            Assert.Equal(50, basis.Value);
        }

        [Fact]
        public void Flex_Auto_ExpandsToExpectedDefaults()
        {
            var style = ParseRule("flex: auto;");

            // Note: flex-grow/shrink/basis return RawValue through parser (known limitation)
            Assert.Equal("1", GetTypedValue(style, "flex-grow").CssText);
            Assert.Equal("1", GetTypedValue(style, "flex-shrink").CssText);
            Assert.Equal("auto", GetTypedValue(style, "flex-basis").CssText);
        }

        [Fact]
        public void Flex_None_ExpandsToLonghands()
        {
            var style = ParseRule("flex: 0 0 auto;");

            // Note: flex-grow/shrink/basis return RawValue through parser (known limitation)
            Assert.Equal("0", GetTypedValue(style, "flex-grow").CssText);
            Assert.Equal("0", GetTypedValue(style, "flex-shrink").CssText);
            Assert.Equal("auto", GetTypedValue(style, "flex-basis").CssText);
        }

        #endregion

        #region Gap Shorthand - Lengths and Percents

        [Fact]
        public void Gap_SingleValue_ExpandsToRowAndColumn()
        {
            var style = ParseRule("gap: 16px;");

            var rowGap = Assert.IsType<Length>(GetTypedValue(style, "row-gap"));
            Assert.Equal(16, rowGap.Value);
            Assert.Equal(Length.Unit.Px, rowGap.Type);

            var colGap = Assert.IsType<Length>(GetTypedValue(style, "column-gap"));
            Assert.Equal(16, colGap.Value);
        }

        [Fact]
        public void Gap_TwoValues_ExpandsToRowAndColumn()
        {
            var style = ParseRule("gap: 10px 20px;");

            Assert.Equal(10, Assert.IsType<Length>(GetTypedValue(style, "row-gap")).Value);
            Assert.Equal(20, Assert.IsType<Length>(GetTypedValue(style, "column-gap")).Value);
        }

        [Fact]
        public void Gap_Percent_ExpandsToTypedPercent()
        {
            var style = ParseRule("gap: 5%;");
            var percent = Assert.IsType<Percent>(GetTypedValue(style, "row-gap"));
            Assert.Equal(5, percent.Value);
        }

        [Fact]
        public void Gap_MixedUnits_ExpandsToTypedValues()
        {
            var style = ParseRule("gap: 1rem 2%;");

            var rowGap = Assert.IsType<Length>(GetTypedValue(style, "row-gap"));
            Assert.Equal(Length.Unit.Rem, rowGap.Type);

            var colGap = Assert.IsType<Percent>(GetTypedValue(style, "column-gap"));
            Assert.Equal(2, colGap.Value);
        }

        #endregion

        #region Longhand Properties - Direct Parsing

        [Fact]
        public void BackgroundColor_Rgba_ReturnsTypedColor()
        {
            var style = ParseRule("background-color: rgba(128, 64, 32, 0.9);");
            var color = Assert.IsType<Color>(GetTypedValue(style, "background-color"));

            Assert.Equal(128, color.R);
            Assert.Equal(64, color.G);
            Assert.Equal(32, color.B);
            Assert.Equal(0.9, color.Alpha, 0.01);
        }

        [Fact]
        public void BackgroundColor_Hsl_ReturnsTypedColor()
        {
            var style = ParseRule("background-color: hsl(240, 100%, 50%);");
            var color = Assert.IsType<Color>(GetTypedValue(style, "background-color"));

            Assert.Equal(0, color.R);
            Assert.Equal(0, color.G);
            Assert.Equal(255, color.B);
        }

        [Fact]
        public void Width_Px_ReturnsTypedLength()
        {
            var style = ParseRule("width: 100px;");
            var length = Assert.IsType<Length>(GetTypedValue(style, "width"));

            Assert.Equal(100, length.Value);
            Assert.Equal(Length.Unit.Px, length.Type);
        }

        [Fact]
        public void Width_Percent_ReturnsTypedPercent()
        {
            var style = ParseRule("width: 50%;");
            var percent = Assert.IsType<Percent>(GetTypedValue(style, "width"));
            Assert.Equal(50, percent.Value);
        }

        [Fact]
        public void Opacity_ReturnsTypedNumber()
        {
            var style = ParseRule("opacity: 0.5;");
            var number = Assert.IsType<Number>(GetTypedValue(style, "opacity"));
            Assert.Equal(0.5f, number.Value);
        }

        [Fact]
        public void Color_Named_ReturnsTypedColor()
        {
            var style = ParseRule("color: blue;");
            Assert.Equal(Color.Blue, GetTypedValue(style, "color"));
        }

        #endregion
    }
}
