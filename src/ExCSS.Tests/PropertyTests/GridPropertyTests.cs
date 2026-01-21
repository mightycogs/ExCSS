using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExCSS.Tests.PropertyTests
{
    public class GridPropertyTests : CssConstructionFunctions
    {
        [Theory]
        [MemberData(nameof(GridTemplateColumnsTestValues))]
        public void GridTemplateColumnsLegalValues(string value)
            => TestForLegalValue<GridTemplateColumnsProperty>(PropertyNames.GridTemplateColumns, value);

        [Theory]
        [MemberData(nameof(GridTemplateRowsTestValues))]
        public void GridTemplateRowsLegalValues(string value)
            => TestForLegalValue<GridTemplateRowsProperty>(PropertyNames.GridTemplateRows, value);

        [Theory]
        [MemberData(nameof(GridPlacementTestValues))]
        public void GridColumnStartLegalValues(string value)
            => TestForLegalValue<GridColumnStartProperty>(PropertyNames.GridColumnStart, value);

        [Theory]
        [MemberData(nameof(GridPlacementTestValues))]
        public void GridColumnEndLegalValues(string value)
            => TestForLegalValue<GridColumnEndProperty>(PropertyNames.GridColumnEnd, value);

        [Theory]
        [MemberData(nameof(GridPlacementTestValues))]
        public void GridRowStartLegalValues(string value)
            => TestForLegalValue<GridRowStartProperty>(PropertyNames.GridRowStart, value);

        [Theory]
        [MemberData(nameof(GridPlacementTestValues))]
        public void GridRowEndLegalValues(string value)
            => TestForLegalValue<GridRowEndProperty>(PropertyNames.GridRowEnd, value);

        [Theory]
        [MemberData(nameof(GridAreaTestValues))]
        public void GridAreaLegalValues(string value)
            => TestForLegalValue<GridAreaProperty>(PropertyNames.GridArea, value);

        [Fact]
        public void GridTemplateColumnsIsNotUnknownProperty()
        {
            var source = ".test { grid-template-columns: 1fr 2fr; }";
            var styleSheet = ParseStyleSheet(source);
            var rule = styleSheet.StyleRules.First() as StyleRule;

            Assert.NotNull(rule);
            Assert.Equal("1fr 2fr", rule.Style.GridTemplateColumns);

            var declarations = rule.Style.Declarations.ToList();
            var gridProp = declarations.FirstOrDefault(d => d.Name == "grid-template-columns");
            Assert.NotNull(gridProp);
            Assert.IsType<GridTemplateColumnsProperty>(gridProp);
        }

        [Fact]
        public void GridTemplateColumnsWithInlineCommentParsesCorrectly()
        {
            var source = @".cyber-dashboard {
                display: grid;
                grid-template-columns: 260px 1fr; /* Sidebar fixed */
                grid-template-rows: auto 1fr auto;
                min-height: 100vh;
            }";
            var styleSheet = ParseStyleSheet(source);
            var rule = styleSheet.StyleRules.First() as StyleRule;

            Assert.NotNull(rule);
            Assert.Equal("260px 1fr", rule.Style.GridTemplateColumns);
            Assert.Equal("auto 1fr auto", rule.Style.GridTemplateRows);
            Assert.Equal("grid", rule.Style.Display);
        }

        [Fact]
        public void LengthUnitFrParsesCorrectly()
        {
            Assert.True(Length.TryParse("1fr", out var length1));
            Assert.Equal(1f, length1.Value);
            Assert.Equal(Length.Unit.Fr, length1.Type);
            Assert.Equal("1fr", length1.ToString());

            Assert.True(Length.TryParse("2.5fr", out var length2));
            Assert.Equal(2.5f, length2.Value);
            Assert.Equal(Length.Unit.Fr, length2.Type);
        }

        [Fact]
        public void GridTemplateRowsIsNotUnknownProperty()
        {
            var source = ".test { grid-template-rows: 100px auto; }";
            var styleSheet = ParseStyleSheet(source);
            var rule = styleSheet.StyleRules.First() as StyleRule;

            Assert.NotNull(rule);
            Assert.Equal("100px auto", rule.Style.GridTemplateRows);

            var declarations = rule.Style.Declarations.ToList();
            var gridProp = declarations.FirstOrDefault(d => d.Name == "grid-template-rows");
            Assert.NotNull(gridProp);
            Assert.IsType<GridTemplateRowsProperty>(gridProp);
        }

        [Fact]
        public void GridPlacementPropertiesParse()
        {
            var source = @".test {
                grid-column-start: 1;
                grid-column-end: 3;
                grid-row-start: 2;
                grid-row-end: 4;
            }";
            var styleSheet = ParseStyleSheet(source);
            var rule = styleSheet.StyleRules.First() as StyleRule;

            Assert.NotNull(rule);
            Assert.Equal("1", rule.Style.GridColumnStart);
            Assert.Equal("3", rule.Style.GridColumnEnd);
            Assert.Equal("2", rule.Style.GridRowStart);
            Assert.Equal("4", rule.Style.GridRowEnd);
        }

        [Fact]
        public void GridTemplateAreasIsNotUnknownProperty()
        {
            var source = ".test { grid-template-areas: 'header header' 'nav main' 'footer footer'; }";
            var styleSheet = ParseStyleSheet(source);
            var rule = styleSheet.StyleRules.First() as StyleRule;

            Assert.NotNull(rule);
            Assert.Equal("\"header header\" \"nav main\" \"footer footer\"", rule.Style.GridTemplateAreas);

            var declarations = rule.Style.Declarations.ToList();
            var gridProp = declarations.FirstOrDefault(d => d.Name == "grid-template-areas");
            Assert.NotNull(gridProp);
            Assert.IsType<GridTemplateAreasProperty>(gridProp);
        }

        [Fact]
        public void GridTemplateAreasMultilineParse()
        {
            var source = @".layout {
                grid-template-areas:
                    'header header'
                    'nav    main'
                    'footer footer';
            }";
            var styleSheet = ParseStyleSheet(source);
            var rule = styleSheet.StyleRules.First() as StyleRule;

            Assert.NotNull(rule);
            var areasValue = rule.Style.GridTemplateAreas;
            Assert.NotNull(areasValue);
            Assert.Contains("header", areasValue);
            Assert.Contains("nav", areasValue);
            Assert.Contains("main", areasValue);
            Assert.Contains("footer", areasValue);
        }

        public static IEnumerable<object[]> GridTemplateColumnsTestValues
        {
            get
            {
                return new[]
                {
                    new object[] { "1fr" },
                    new object[] { "1fr 2fr" },
                    new object[] { "100px auto" },
                    new object[] { "1fr 100px 2fr" },
                    new object[] { "100px" },
                    new object[] { "auto" },
                    new object[] { "none" },
                    new object[] { "repeat(3, 1fr)" },
                    new object[] { "minmax(100px, 1fr)" },
                }.Union(GlobalKeywordTestValues.ToObjectArray(), ObjectArrayComparer.Instance);
            }
        }

        public static IEnumerable<object[]> GridTemplateRowsTestValues
        {
            get
            {
                return new[]
                {
                    new object[] { "1fr" },
                    new object[] { "100px auto" },
                    new object[] { "50px 100px" },
                    new object[] { "auto" },
                    new object[] { "none" },
                    new object[] { "minmax(50px, auto)" },
                }.Union(GlobalKeywordTestValues.ToObjectArray(), ObjectArrayComparer.Instance);
            }
        }

        public static IEnumerable<object[]> GridPlacementTestValues
        {
            get
            {
                return new[]
                {
                    new object[] { "1" },
                    new object[] { "2" },
                    new object[] { "-1" },
                    new object[] { "auto" },
                    new object[] { "header" },
                }.Union(GlobalKeywordTestValues.ToObjectArray(), ObjectArrayComparer.Instance);
            }
        }

        public static IEnumerable<object[]> GridAreaTestValues
        {
            get
            {
                return new[]
                {
                    new object[] { "auto" },
                    new object[] { "header" },
                    new object[] { "1/1/2/2" },
                    new object[] { "2/1/3/-1" },
                }.Union(GlobalKeywordTestValues.ToObjectArray(), ObjectArrayComparer.Instance);
            }
        }
    }
}
