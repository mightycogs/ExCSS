using System.Linq;
using Xunit;

namespace ExCSS.Tests.ExtendedTestsPart1
{
    public class ResilienceTests : ExtendedTestBase
    {
        protected Stylesheet ParseSheet(string css)
        {
            return Parser.Parse(css);
        }

        [Fact]
        public void Parse_MissingSemicolon_RecoverNextProperty()
        {
            var css = ".test { color: red background: blue; }";
            var sheet = ParseSheet(css);

            Assert.NotNull(sheet);
            Assert.Single(sheet.StyleRules);
            var rule = sheet.StyleRules.First();
            Assert.NotNull(rule.Style);
        }

        [Fact]
        public void Parse_UnclosedBrace_DoesNotCrash()
        {
            var css = ".test { color: red;";
            var sheet = ParseSheet(css);

            Assert.NotNull(sheet);
        }

        [Fact]
        public void Parse_InvalidPropertyValue_PropertyDroppedOrRaw()
        {
            var css = ".test { width: notavalidunit; height: 100px; }";
            var sheet = ParseSheet(css);

            Assert.NotNull(sheet);
            Assert.Single(sheet.StyleRules);
            var rule = sheet.StyleRules.First();
            var heightProp = rule.Style.GetProperty("height");
            Assert.NotNull(heightProp);
            Assert.Equal("100px", heightProp.Value);
        }

        [Fact]
        public void Parse_MalformedCalc_DoesNotCrash()
        {
            var css = ".test { width: calc(100% - ; height: 50px; }";
            var sheet = ParseSheet(css);

            Assert.NotNull(sheet);
            Assert.Single(sheet.StyleRules);
        }

        [Fact]
        public void Parse_UnterminatedString_Recovers()
        {
            var css = ".test { content: \"unterminated; color: red; }";
            var sheet = ParseSheet(css);

            Assert.NotNull(sheet);
        }

        [Fact]
        public void Parse_EmptyRuleBody_ParsesSuccessfully()
        {
            var css = ".test { }";
            var sheet = ParseSheet(css);

            Assert.NotNull(sheet);
            Assert.Single(sheet.StyleRules);
            var rule = sheet.StyleRules.First();
            Assert.NotNull(rule.Style);
        }

        [Fact]
        public void Parse_MissingColonInDeclaration_SkipsMalformedDeclaration()
        {
            var css = ".test { color red; background: blue; }";
            var sheet = ParseSheet(css);

            Assert.NotNull(sheet);
            Assert.Single(sheet.StyleRules);
            var rule = sheet.StyleRules.First();
            var bgProp = rule.Style.GetProperty("background");
            Assert.NotNull(bgProp);
            Assert.True(bgProp.Value == "blue" || bgProp.Value == "rgb(0, 0, 255)");
        }

        [Fact]
        public void Parse_DoubleSemicolons_HandlesGracefully()
        {
            var css = ".test { color: red;; background: blue; }";
            var sheet = ParseSheet(css);

            Assert.NotNull(sheet);
            Assert.Single(sheet.StyleRules);
            var rule = sheet.StyleRules.First();
            var colorProp = rule.Style.GetProperty("color");
            Assert.NotNull(colorProp);
            Assert.True(colorProp.Value == "red" || colorProp.Value == "rgb(255, 0, 0)");
            var bgProp = rule.Style.GetProperty("background");
            Assert.NotNull(bgProp);
            Assert.True(bgProp.Value == "blue" || bgProp.Value == "rgb(0, 0, 255)");
        }

        [Fact]
        public void Parse_MultipleRulesWithErrors_RecoversBetweenRules()
        {
            var css = @"
                .valid1 { color: red; }
                .broken { width: calc( }
                .valid2 { background: blue; }
            ";
            var sheet = ParseSheet(css);

            Assert.NotNull(sheet);
            Assert.True(sheet.StyleRules.Count() >= 1);
        }

        [Fact]
        public void Parse_NestedBracesInvalid_DoesNotCrash()
        {
            var css = ".test { color: rgb(255, 0, 0; }";
            var sheet = ParseSheet(css);

            Assert.NotNull(sheet);
        }

        [Fact]
        public void Parse_EmptySelector_DoesNotCrash()
        {
            var css = "{ color: red; }";
            var sheet = ParseSheet(css);

            Assert.NotNull(sheet);
        }

        [Fact]
        public void Parse_OnlyWhitespace_ReturnsEmptyStylesheet()
        {
            var css = "   \n\t  \n  ";
            var sheet = ParseSheet(css);

            Assert.NotNull(sheet);
            Assert.Empty(sheet.StyleRules);
        }

        [Fact]
        public void Parse_EmptyString_ReturnsEmptyStylesheet()
        {
            var css = "";
            var sheet = ParseSheet(css);

            Assert.NotNull(sheet);
            Assert.Empty(sheet.StyleRules);
        }
    }
}
