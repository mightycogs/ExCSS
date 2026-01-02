using System.Linq;
using Xunit;

namespace ExCSS.Tests.ExtendedTestsPart1
{
    public class NewFeaturesTests : ExtendedTestBase
    {
        private Stylesheet Parse(string css)
        {
            var parser = new StylesheetParser();
            return parser.Parse(css);
        }

        [Fact]
        public void Parse_AspectRatio_Ratio_IsRecognized()
        {
            var sheet = Parse(".test { aspect-ratio: 16 / 9; }");
            var rule = sheet.Rules.OfType<StyleRule>().First();
            var prop = rule.Style.GetProperty("aspect-ratio");

            Assert.NotNull(prop);
            Assert.True(prop.HasValue);
            
            var list = prop.TypedValue as StyleValueList;
            Assert.NotNull(list);
            Assert.Equal(2, list.Count);
            
            Assert.IsType<Number>(list[0]);
            var width = (Number)list[0];
            Assert.Equal(16, width.Value);
            
            Assert.IsType<Number>(list[1]);
            var height = (Number)list[1];
            Assert.Equal(9, height.Value);
        }

        [Fact]
        public void Parse_AspectRatio_Number_IsRecognized()
        {
            var sheet = Parse(".test { aspect-ratio: 1.5; }");
            var rule = sheet.Rules.OfType<StyleRule>().First();
            var prop = rule.Style.GetProperty("aspect-ratio");

            Assert.NotNull(prop);
            Assert.True(prop.HasValue);
            Assert.Equal("1.5", prop.Value);
            
            Assert.IsType<Number>(prop.TypedValue);
            var num = (Number)prop.TypedValue;
            Assert.Equal(1.5, num.Value);
        }

        [Fact]
        public void Parse_AspectRatio_Auto_IsRecognized()
        {
            var sheet = Parse(".test { aspect-ratio: auto; }");
            var rule = sheet.Rules.OfType<StyleRule>().First();
            var prop = rule.Style.GetProperty("aspect-ratio");

            Assert.NotNull(prop);
            Assert.True(prop.HasValue);
            Assert.Equal("auto", prop.Value);
        }

        [Fact]
        public void Parse_BackdropFilter_Blur_IsFunction()
        {
            var sheet = Parse(".test { backdrop-filter: blur(10px); }");
            var rule = sheet.Rules.OfType<StyleRule>().First();
            var prop = rule.Style.GetProperty("backdrop-filter");

            Assert.NotNull(prop);
            Assert.True(prop.HasValue);
            
            // Can be a single function or a list. For a single item, ExCSS often unwraps the list.
            if (prop.TypedValue is StyleValueList list)
            {
                Assert.Single(list);
                Assert.IsType<FunctionValue>(list[0]);
                var function = (FunctionValue)list[0];
                Assert.Equal("blur", function.Name);
                Assert.Equal("10px", function.Arguments);
            }
            else
            {
                Assert.IsType<FunctionValue>(prop.TypedValue);
                var function = (FunctionValue)prop.TypedValue;
                Assert.Equal("blur", function.Name);
                Assert.Equal("10px", function.Arguments);
            }
        }
    }
}
