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
            Assert.Equal(2, complex.Length);

            var firstPart = complex.First();
            Assert.IsType<CompoundSelector>(firstPart.Selector);
            var compound = (CompoundSelector)firstPart.Selector;

            var hasPartCard = compound.OfType<ClassSelector>().Any(c => c.Class == "part-card");
            Assert.True(hasPartCard);

            var hasHover = compound.OfType<PseudoClassSelector>().Any(p => p.Class == "hover");
            Assert.True(hasHover);

            Assert.Equal(" ", firstPart.Delimiter);

            var secondPart = complex.Skip(1).First();
            Assert.IsType<ClassSelector>(secondPart.Selector);
            Assert.Equal("part-thumbnail", ((ClassSelector)secondPart.Selector).Class);
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
            var complex = (ComplexSelector)rule.Selector;
            Assert.Equal(2, complex.Length);

            var firstPart = complex.First();
            Assert.IsType<ClassSelector>(firstPart.Selector);
            Assert.Equal("settings-section", ((ClassSelector)firstPart.Selector).Class);
            Assert.Equal(" ", firstPart.Delimiter);

            var secondPart = complex.Skip(1).First();
            Assert.IsType<CompoundSelector>(secondPart.Selector);
            var compound = (CompoundSelector)secondPart.Selector;
            Assert.Equal(2, compound.Length);

            Assert.IsType<ClassSelector>(compound[0]);
            Assert.Equal("setting-row", ((ClassSelector)compound[0]).Class);

            var nthChild = compound.OfType<FirstChildSelector>().FirstOrDefault();
            Assert.NotNull(nthChild);
            Assert.Equal(2, nthChild.Step);
            Assert.Equal(1, nthChild.Offset);

            Assert.Contains(":nth-child(2n+1)", rule.Selector.Text);
        }

        [Fact]
        public void Parse_FirstChild_HasPseudoClass()
        {
            var sheet = ParseFixture("Selectors", "006_first_last_child.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<CompoundSelector>(rule.Selector);
            var compound = (CompoundSelector)rule.Selector;
            Assert.Equal(2, compound.Length);

            Assert.IsType<ClassSelector>(compound[0]);
            Assert.Equal("toggle-btn", ((ClassSelector)compound[0]).Class);

            Assert.IsType<PseudoClassSelector>(compound[1]);
            Assert.Equal("first-child", ((PseudoClassSelector)compound[1]).Class);

            Assert.Contains("first-child", rule.Selector.Text);
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

            Assert.IsType<ComplexSelector>(rule.Selector);
            var complex = (ComplexSelector)rule.Selector;

            var firstPart = complex.First();
            Assert.IsType<CompoundSelector>(firstPart.Selector);
            var compound = (CompoundSelector)firstPart.Selector;

            var notSelectors = compound.OfType<NotSelector>().ToList();
            Assert.Equal(2, notSelectors.Count);

            var innerClasses = notSelectors.Select(n => n.InnerSelector).OfType<ClassSelector>().Select(c => c.Class).ToList();
            Assert.Contains("locked", innerClasses);
            Assert.Contains("selected", innerClasses);

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
            var compound = (CompoundSelector)rule.Selector;
            Assert.Equal(2, compound.Length);

            Assert.IsType<PseudoElementSelector>(compound[0]);
            Assert.Equal("-webkit-scrollbar-thumb", ((PseudoElementSelector)compound[0]).Name);

            Assert.IsType<PseudoClassSelector>(compound[1]);
            Assert.Equal("hover", ((PseudoClassSelector)compound[1]).Class);

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
            var compound = (CompoundSelector)rule.Selector;
            Assert.Equal(2, compound.Length);

            Assert.IsType<ClassSelector>(compound[0]);
            Assert.Equal("slider", ((ClassSelector)compound[0]).Class);

            Assert.IsType<PseudoElementSelector>(compound[1]);
            Assert.Equal("-webkit-slider-thumb", ((PseudoElementSelector)compound[1]).Name);

            Assert.Contains("-webkit-slider-thumb", rule.Selector.Text);
        }

        [Fact]
        public void Parse_MozRangeThumb_CompoundSelector()
        {
            var sheet = ParseFixture("Selectors", "012_moz_range_thumb.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<CompoundSelector>(rule.Selector);
            var compound = (CompoundSelector)rule.Selector;
            Assert.Equal(2, compound.Length);

            Assert.IsType<ClassSelector>(compound[0]);
            Assert.Equal("slider", ((ClassSelector)compound[0]).Class);

            Assert.IsType<PseudoElementSelector>(compound[1]);
            Assert.Equal("-moz-range-thumb", ((PseudoElementSelector)compound[1]).Name);

            Assert.Contains("-moz-range-thumb", rule.Selector.Text);
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

            Assert.IsType<CompoundSelector>(rule.Selector);
            var compound = (CompoundSelector)rule.Selector;
            Assert.Equal(2, compound.Length);

            Assert.IsType<ClassSelector>(compound[0]);
            Assert.Equal("cheat-input", ((ClassSelector)compound[0]).Class);

            Assert.IsType<PseudoClassSelector>(compound[1]);
            Assert.Equal("focus", ((PseudoClassSelector)compound[1]).Class);

            Assert.Contains(":focus", rule.Selector.Text);
        }

        [Fact]
        public void Parse_LastChild_HasPseudoClass()
        {
            var sheet = ParseFixture("Selectors", "016_last_child.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<CompoundSelector>(rule.Selector);
            var compound = (CompoundSelector)rule.Selector;
            Assert.Equal(2, compound.Length);

            Assert.IsType<ClassSelector>(compound[0]);
            Assert.Equal("toggle-btn", ((ClassSelector)compound[0]).Class);

            Assert.IsType<PseudoClassSelector>(compound[1]);
            Assert.Equal("last-child", ((PseudoClassSelector)compound[1]).Class);

            Assert.Contains("last-child", rule.Selector.Text);
        }

        [Fact]
        public void Parse_NthChildN_ComplexSelector()
        {
            var sheet = ParseFixture("Selectors", "017_nth_child_n.css");
            var rule = GetSingleStyleRule(sheet);

            Assert.IsType<ComplexSelector>(rule.Selector);
            var complex = (ComplexSelector)rule.Selector;
            Assert.Equal(2, complex.Length);

            var firstPart = complex.First();
            Assert.IsType<ClassSelector>(firstPart.Selector);
            Assert.Equal("shop-grid-big", ((ClassSelector)firstPart.Selector).Class);
            Assert.Equal(">", firstPart.Delimiter);

            var secondPart = complex.Skip(1).First();
            Assert.IsType<CompoundSelector>(secondPart.Selector);
            var compound = (CompoundSelector)secondPart.Selector;

            var nthChild = compound.OfType<FirstChildSelector>().FirstOrDefault();
            Assert.NotNull(nthChild);
            Assert.Equal(1, nthChild.Step);
            Assert.Equal(0, nthChild.Offset);

            Assert.Contains(":nth-child(n)", rule.Selector.Text);
        }
    }
}
