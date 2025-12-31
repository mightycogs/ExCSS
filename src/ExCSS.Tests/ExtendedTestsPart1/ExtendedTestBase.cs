using System.IO;
using System.Linq;
using Xunit;

namespace ExCSS.Tests.ExtendedTestsPart1
{
    public abstract class ExtendedTestBase
    {
        protected readonly StylesheetParser Parser;
        protected readonly string FixturesPath;

        protected ExtendedTestBase()
        {
            Parser = new StylesheetParser();
            var assemblyPath = Path.GetDirectoryName(typeof(ExtendedTestBase).Assembly.Location);
            FixturesPath = Path.Combine(assemblyPath, "ExtendedTestsPart1", "Fixtures");
        }

        protected string LoadFixture(string category, string filename)
        {
            var path = Path.Combine(FixturesPath, category, filename);
            return File.ReadAllText(path);
        }

        protected Stylesheet ParseFixture(string category, string filename)
        {
            var css = LoadFixture(category, filename);
            return Parser.Parse(css);
        }

        protected IStyleRule GetSingleStyleRule(Stylesheet sheet)
        {
            Assert.Single(sheet.StyleRules);
            return sheet.StyleRules.First();
        }
    }
}
