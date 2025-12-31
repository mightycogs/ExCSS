using System.Linq;
using Xunit;

namespace ExCSS.Tests.ExtendedTestsPart1
{
    public class SelectorTests : ExtendedTestBase
    {
        [Fact]
        public void Parse_RootHtmlBodyId_ListSelector()
        {
            var sheet = ParseFixture("Selectors", "001_root_html_body_id.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<ListSelector>(rule.Selector);
            var listSelector = (ListSelector)rule.Selector;
            Assert.Equal(4, listSelector.Length);
        }

        [Fact]
        public void Parse_AdjacentSiblingUniversal_ComplexSelector()
        {
            var sheet = ParseFixture("Selectors", "002_adjacent_sibling_universal.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<ComplexSelector>(rule.Selector);
            var complex = (ComplexSelector)rule.Selector;
            Assert.Equal(3, complex.Length);
        }

        [Fact]
        public void Parse_HoverMultipleNot_ComplexSelector()
        {
            var sheet = ParseFixture("Selectors", "003_hover_multiple_not.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<ComplexSelector>(rule.Selector);
            var complex = (ComplexSelector)rule.Selector;
            Assert.True(complex.Length >= 2);
        }

        [Fact]
        public void Parse_MultipleSelectorsDescendant_ListSelector()
        {
            var sheet = ParseFixture("Selectors", "004_multiple_selectors_descendant.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<ListSelector>(rule.Selector);
            var listSelector = (ListSelector)rule.Selector;
            Assert.Equal(2, listSelector.Length);
        }

        [Fact]
        public void Parse_NthChild_ComplexSelector()
        {
            var sheet = ParseFixture("Selectors", "005_nth_child.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<ComplexSelector>(rule.Selector);
        }

        [Fact]
        public void Parse_FirstChild_HasPseudoClass()
        {
            var sheet = ParseFixture("Selectors", "006_first_last_child.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.NotNull(rule.Selector);
            var selectorText = rule.Selector.Text;
            Assert.Contains("first-child", selectorText);
        }

        [Fact]
        public void Parse_WebkitPseudoElement_PseudoElementSelector()
        {
            var sheet = ParseFixture("Selectors", "007_webkit_pseudo_element.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<PseudoElementSelector>(rule.Selector);
            var pseudoElement = (PseudoElementSelector)rule.Selector;
            Assert.Equal("-webkit-scrollbar", pseudoElement.Name);
        }

        [Fact]
        public void Parse_PseudoAfter_CompoundSelectorWithPseudoElement()
        {
            var sheet = ParseFixture("Selectors", "008_pseudo_after.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<CompoundSelector>(rule.Selector);
            var compound = (CompoundSelector)rule.Selector;
            Assert.Equal(3, compound.Length);

            Assert.IsType<ClassSelector>(compound[0]);
            Assert.Equal("part-card", ((ClassSelector)compound[0]).Class);

            Assert.IsType<ClassSelector>(compound[1]);
            Assert.Equal("selected", ((ClassSelector)compound[1]).Class);

            Assert.IsType<PseudoElementSelector>(compound[2]);
            Assert.Equal("after", ((PseudoElementSelector)compound[2]).Name);
        }

        [Fact]
        public void Parse_NotSelector_ContainsInnerSelector()
        {
            var sheet = ParseFixture("Selectors", "003_hover_multiple_not.css");
            var rule = GetSingleStyleRule(sheet);

            var selectorText = rule.Selector.Text;
            Assert.Contains(":not(.locked)", selectorText);
            Assert.Contains(":not(.selected)", selectorText);
        }

        [Fact]
        public void Parse_WebkitScrollbarTrack_PseudoElementSelector()
        {
            var sheet = ParseFixture("Selectors", "009_webkit_scrollbar_track.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<PseudoElementSelector>(rule.Selector);
            var pseudoElement = (PseudoElementSelector)rule.Selector;
            Assert.Equal("-webkit-scrollbar-track", pseudoElement.Name);
        }

        [Fact]
        public void Parse_WebkitScrollbarThumbHover_CompoundSelector()
        {
            var sheet = ParseFixture("Selectors", "010_webkit_scrollbar_thumb_hover.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<CompoundSelector>(rule.Selector);
            var selectorText = rule.Selector.Text;
            Assert.Contains("-webkit-scrollbar-thumb", selectorText);
            Assert.Contains(":hover", selectorText);
        }

        [Fact]
        public void Parse_WebkitSliderThumb_CompoundSelector()
        {
            var sheet = ParseFixture("Selectors", "011_webkit_slider_thumb.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<CompoundSelector>(rule.Selector);
            var selectorText = rule.Selector.Text;
            Assert.Contains("-webkit-slider-thumb", selectorText);
        }

        [Fact]
        public void Parse_MozRangeThumb_CompoundSelector()
        {
            var sheet = ParseFixture("Selectors", "012_moz_range_thumb.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<CompoundSelector>(rule.Selector);
            var selectorText = rule.Selector.Text;
            Assert.Contains("-moz-range-thumb", selectorText);
        }

        [Fact]
        public void Parse_WebkitSpinButton_ListSelector()
        {
            var sheet = ParseFixture("Selectors", "013_webkit_spin_button.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<ListSelector>(rule.Selector);
            var listSelector = (ListSelector)rule.Selector;
            Assert.Equal(2, listSelector.Length);

            var selectorText = rule.Selector.Text;
            Assert.Contains("-webkit-outer-spin-button", selectorText);
            Assert.Contains("-webkit-inner-spin-button", selectorText);
        }

        [Fact]
        public void Parse_PseudoBefore_CompoundSelectorWithPseudoElement()
        {
            var sheet = ParseFixture("Selectors", "014_pseudo_before.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<CompoundSelector>(rule.Selector);
            var compound = (CompoundSelector)rule.Selector;

            var hasBefore = compound.OfType<PseudoElementSelector>().Any(p => p.Name == "before");
            Assert.True(hasBefore);
        }

        [Fact]
        public void Parse_Focus_HasFocusPseudoClass()
        {
            var sheet = ParseFixture("Selectors", "015_focus.css");
            var rule = GetSingleStyleRule(sheet);

            var selectorText = rule.Selector.Text;
            Assert.Contains(":focus", selectorText);
        }

        [Fact]
        public void Parse_LastChild_HasPseudoClass()
        {
            var sheet = ParseFixture("Selectors", "016_last_child.css");
            var rule = GetSingleStyleRule(sheet);

            var selectorText = rule.Selector.Text;
            Assert.Contains("last-child", selectorText);
        }

        [Fact]
        public void Parse_NthChildN_ComplexSelector()
        {
            var sheet = ParseFixture("Selectors", "017_nth_child_n.css");
            var rule = GetSingleStyleRule(sheet);

            var selectorText = rule.Selector.Text;
            Assert.Contains(":nth-child(n)", selectorText);
        }
    }
}
