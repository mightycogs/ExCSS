namespace ExCSS
{
    public sealed class MatchesSelector : SelectorBase
    {
        internal MatchesSelector(ISelector innerSelector)
            : base(innerSelector.Specificity, PseudoClassNames.Matches.StylesheetFunction(innerSelector.Text))
        {
            InnerSelector = innerSelector;
        }

        public ISelector InnerSelector { get; }
    }
}
