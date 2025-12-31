using System.Linq;
using Xunit;

namespace ExCSS.Tests.ExtendedTestsPart1
{
    public class PropertyTests : ExtendedTestBase
    {
        [Fact]
        public void Parse_CustomProperty_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "001_custom_property.css");
            var rule = GetSingleStyleRule(sheet);
            var customProp = rule.Style.GetProperty("--bg-dark");

            Assert.NotNull(customProp);
            Assert.Equal("--bg-dark", customProp.Name);
            Assert.True(customProp.HasValue);
        }

        [Fact]
        public void Parse_CustomProperty_ValueIsColor()
        {
            var sheet = ParseFixture("Properties", "001_custom_property.css");
            var rule = GetSingleStyleRule(sheet);
            var customProp = rule.Style.GetProperty("--bg-dark");

            Assert.NotNull(customProp);
            var typedValue = customProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsType<Color>(typedValue);
        }

        [Fact]
        public void Parse_InsetShorthand_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "002_inset_shorthand.css");
            var rule = GetSingleStyleRule(sheet);
            var insetProp = rule.Style.GetProperty("inset");

            Assert.NotNull(insetProp);
            Assert.Equal("inset", insetProp.Name);
            Assert.True(insetProp.HasValue);
        }

        [Fact]
        public void Parse_InsetShorthand_NotRawValue()
        {
            var sheet = ParseFixture("Properties", "002_inset_shorthand.css");
            var rule = GetSingleStyleRule(sheet);
            var insetProp = rule.Style.GetProperty("inset");

            Assert.NotNull(insetProp);
            var typedValue = insetProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
        }

        [Fact]
        public void Parse_BorderShorthand_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "003_border_shorthand.css");
            var rule = GetSingleStyleRule(sheet);
            var borderProp = rule.Style.GetProperty("border");

            Assert.NotNull(borderProp);
            Assert.Equal("border", borderProp.Name);
            Assert.True(borderProp.HasValue);
        }

        [Fact]
        public void Parse_BorderShorthand_NotRawValue()
        {
            var sheet = ParseFixture("Properties", "003_border_shorthand.css");
            var rule = GetSingleStyleRule(sheet);
            var borderProp = rule.Style.GetProperty("border");

            Assert.NotNull(borderProp);
            var typedValue = borderProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
        }

        [Fact]
        public void Parse_BackdropFilter_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "004_backdrop_filter.css");
            var rule = GetSingleStyleRule(sheet);
            var filterProp = rule.Style.GetProperty("backdrop-filter");

            Assert.NotNull(filterProp);
            Assert.Equal("backdrop-filter", filterProp.Name);
            Assert.True(filterProp.HasValue);
        }

        [Fact]
        public void Parse_VendorWebkit_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "005_vendor_webkit.css");
            var rule = GetSingleStyleRule(sheet);
            var filterProp = rule.Style.GetProperty("-webkit-backdrop-filter");

            Assert.NotNull(filterProp);
            Assert.Equal("-webkit-backdrop-filter", filterProp.Name);
            Assert.True(filterProp.HasValue);
        }

        [Fact]
        public void Parse_VendorMoz_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "006_vendor_moz.css");
            var rule = GetSingleStyleRule(sheet);
            var appearanceProp = rule.Style.GetProperty("-moz-appearance");

            Assert.NotNull(appearanceProp);
            Assert.Equal("-moz-appearance", appearanceProp.Name);
            Assert.True(appearanceProp.HasValue);
        }

        [Fact]
        public void Parse_BorderInShorthand_ContainsVarValue()
        {
            var sheet = ParseFixture("Properties", "003_border_shorthand.css");
            var rule = GetSingleStyleRule(sheet);
            var borderProp = rule.Style.GetProperty("border");

            Assert.NotNull(borderProp);
            Assert.Contains("var(--border-color)", borderProp.Value);
        }

        [Fact]
        public void Parse_WebkitAppearance_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "007_webkit_appearance.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("-webkit-appearance");

            Assert.NotNull(prop);
            Assert.Equal("-webkit-appearance", prop.Name);
            Assert.Equal("none", prop.Value);
        }

        [Fact]
        public void Parse_Appearance_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "007_webkit_appearance.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("appearance");

            Assert.NotNull(prop);
            Assert.Equal("appearance", prop.Name);
            Assert.Equal("none", prop.Value);
        }

        [Fact]
        public void Parse_WebkitLineClamp_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "008_webkit_line_clamp.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("-webkit-line-clamp");

            Assert.NotNull(prop);
            Assert.Equal("-webkit-line-clamp", prop.Name);
            Assert.True(prop.HasValue);
        }

        [Fact]
        public void Parse_WebkitBoxOrient_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "008_webkit_line_clamp.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("-webkit-box-orient");

            Assert.NotNull(prop);
            Assert.Equal("-webkit-box-orient", prop.Name);
            Assert.Equal("vertical", prop.Value);
        }

        [Fact]
        public void Parse_WebkitFontSmoothing_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "009_webkit_font_smoothing.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("-webkit-font-smoothing");

            Assert.NotNull(prop);
            Assert.Equal("-webkit-font-smoothing", prop.Name);
            Assert.Equal("antialiased", prop.Value);
        }

        [Fact]
        public void Parse_MozOsxFontSmoothing_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "009_webkit_font_smoothing.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("-moz-osx-font-smoothing");

            Assert.NotNull(prop);
            Assert.Equal("-moz-osx-font-smoothing", prop.Name);
            Assert.Equal("grayscale", prop.Value);
        }

        [Fact]
        public void Parse_Animation_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "010_animation.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("animation");

            Assert.NotNull(prop);
            Assert.Equal("animation", prop.Name);
            Assert.True(prop.HasValue);
            Assert.Contains("kenburns", prop.Value);
        }

        [Fact]
        public void Parse_DisplayFlex_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "011_display_flex.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("display");

            Assert.NotNull(prop);
            Assert.Equal("display", prop.Name);
            Assert.Equal("flex", prop.Value);
        }

        [Fact]
        public void Parse_AlignItems_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "011_display_flex.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("align-items");

            Assert.NotNull(prop);
            Assert.Equal("align-items", prop.Name);
            Assert.Equal("center", prop.Value);
        }

        [Fact]
        public void Parse_JustifyContent_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "011_display_flex.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("justify-content");

            Assert.NotNull(prop);
            Assert.Equal("justify-content", prop.Name);
            Assert.Equal("center", prop.Value);
        }

        [Fact]
        public void Parse_FlexShorthand_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "012_flex_shorthand.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("flex");

            Assert.NotNull(prop);
            Assert.Equal("flex", prop.Name);
            Assert.True(prop.HasValue);
        }

        [Fact]
        public void Parse_FlexDirection_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "012_flex_shorthand.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("flex-direction");

            Assert.NotNull(prop);
            Assert.Equal("flex-direction", prop.Name);
            Assert.Equal("column", prop.Value);
        }

        [Fact]
        public void Parse_FlexWrap_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "013_flex_properties.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("flex-wrap");

            Assert.NotNull(prop);
            Assert.Equal("flex-wrap", prop.Name);
            Assert.Equal("wrap", prop.Value);
        }

        [Fact]
        public void Parse_AlignContent_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "013_flex_properties.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("align-content");

            Assert.NotNull(prop);
            Assert.Equal("align-content", prop.Name);
            Assert.Equal("flex-start", prop.Value);
        }

        [Fact]
        public void Parse_BoxSizing_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "014_box_sizing.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("box-sizing");

            Assert.NotNull(prop);
            Assert.Equal("box-sizing", prop.Name);
            Assert.Equal("border-box", prop.Value);
        }

        [Fact]
        public void Parse_ZIndex_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "014_box_sizing.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("z-index");

            Assert.NotNull(prop);
            Assert.Equal("z-index", prop.Name);
            Assert.True(prop.HasValue);
        }

        [Fact]
        public void Parse_Cursor_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "015_cursor.css");
            var rules = sheet.Rules.OfType<StyleRule>().ToList();
            Assert.Equal(2, rules.Count);

            var pointerProp = rules[0].Style.GetProperty("cursor");
            Assert.NotNull(pointerProp);
            Assert.Equal("pointer", pointerProp.Value);

            var notAllowedProp = rules[1].Style.GetProperty("cursor");
            Assert.NotNull(notAllowedProp);
            Assert.Equal("not-allowed", notAllowedProp.Value);
        }

        [Fact]
        public void Parse_OverflowXY_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "016_overflow.css");
            var rule = GetSingleStyleRule(sheet);

            var overflowY = rule.Style.GetProperty("overflow-y");
            Assert.NotNull(overflowY);
            Assert.Equal("auto", overflowY.Value);

            var overflowX = rule.Style.GetProperty("overflow-x");
            Assert.NotNull(overflowX);
            Assert.Equal("visible", overflowX.Value);
        }

        [Fact]
        public void Parse_Visibility_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "017_visibility.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("visibility");

            Assert.NotNull(prop);
            Assert.Equal("visibility", prop.Name);
            Assert.Equal("hidden", prop.Value);
        }

        [Fact]
        public void Parse_Opacity_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "018_opacity.css");
            var rules = sheet.Rules.OfType<StyleRule>().ToList();
            Assert.Equal(2, rules.Count);

            var opacity0 = rules[0].Style.GetProperty("opacity");
            Assert.NotNull(opacity0);
            Assert.Equal("0", opacity0.Value);

            var opacity1 = rules[1].Style.GetProperty("opacity");
            Assert.NotNull(opacity1);
            Assert.Equal("1", opacity1.Value);
        }

        [Fact]
        public void Parse_TextTransform_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "019_text_transform.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("text-transform");

            Assert.NotNull(prop);
            Assert.Equal("text-transform", prop.Name);
            Assert.Equal("uppercase", prop.Value);
        }

        [Fact]
        public void Parse_LetterSpacing_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "019_text_transform.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("letter-spacing");

            Assert.NotNull(prop);
            Assert.Equal("letter-spacing", prop.Name);
            Assert.True(prop.HasValue);
        }

        [Fact]
        public void Parse_WhiteSpace_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "020_white_space.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("white-space");

            Assert.NotNull(prop);
            Assert.Equal("white-space", prop.Name);
            Assert.Equal("nowrap", prop.Value);
        }

        [Fact]
        public void Parse_PointerEvents_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "021_pointer_events.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("pointer-events");

            Assert.NotNull(prop);
            Assert.Equal("pointer-events", prop.Name);
            Assert.Equal("none", prop.Value);
        }

        [Fact]
        public void Parse_UserSelect_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "022_user_select.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("user-select");

            Assert.NotNull(prop);
            Assert.Equal("user-select", prop.Name);
            Assert.Equal("none", prop.Value);
        }

        [Fact]
        public void Parse_ObjectFit_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "023_object_fit.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("object-fit");

            Assert.NotNull(prop);
            Assert.Equal("object-fit", prop.Name);
            Assert.Equal("contain", prop.Value);
        }

        [Fact]
        public void Parse_Outline_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "024_outline.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("outline");

            Assert.NotNull(prop);
            Assert.Equal("outline", prop.Name);
            Assert.Equal("none", prop.Value);
        }

        [Fact]
        public void Parse_TransformOrigin_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "025_transform_origin.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("transform-origin");

            Assert.NotNull(prop);
            Assert.Equal("transform-origin", prop.Name);
            Assert.True(prop.HasValue);
        }

        [Fact]
        public void Parse_BorderSides_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "026_border_sides.css");
            var rule = GetSingleStyleRule(sheet);

            var borderTop = rule.Style.GetProperty("border-top");
            Assert.NotNull(borderTop);
            Assert.True(borderTop.HasValue);

            var borderBottom = rule.Style.GetProperty("border-bottom");
            Assert.NotNull(borderBottom);
            Assert.True(borderBottom.HasValue);

            var borderLeft = rule.Style.GetProperty("border-left");
            Assert.NotNull(borderLeft);
            Assert.True(borderLeft.HasValue);
        }

        [Fact]
        public void Parse_MinWidth_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "027_min_max_dimensions.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("min-width");

            Assert.NotNull(prop);
            Assert.Equal("min-width", prop.Name);
            Assert.True(prop.HasValue);
        }

        [Fact]
        public void Parse_TextAlign_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "028_text_align.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("text-align");

            Assert.NotNull(prop);
            Assert.Equal("text-align", prop.Name);
            Assert.Equal("center", prop.Value);
        }

        [Fact]
        public void Parse_AlignSelf_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "029_align_self.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("align-self");

            Assert.NotNull(prop);
            Assert.Equal("align-self", prop.Name);
            Assert.Equal("stretch", prop.Value);
        }

        [Fact]
        public void Parse_FlexShrink_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "029_align_self.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("flex-shrink");

            Assert.NotNull(prop);
            Assert.Equal("flex-shrink", prop.Name);
            Assert.True(prop.HasValue);
        }

        [Fact]
        public void Parse_MinHeight_IsRecognized()
        {
            var sheet = ParseFixture("Properties", "029_align_self.css");
            var rule = GetSingleStyleRule(sheet);
            var prop = rule.Style.GetProperty("min-height");

            Assert.NotNull(prop);
            Assert.Equal("min-height", prop.Name);
            Assert.True(prop.HasValue);
        }
    }
}
