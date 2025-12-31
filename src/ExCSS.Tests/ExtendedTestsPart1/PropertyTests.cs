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
    }
}
