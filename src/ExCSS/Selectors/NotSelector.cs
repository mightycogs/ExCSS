namespace ExCSS
{
    public sealed class NotSelector : SelectorBase
    {
        internal NotSelector(ISelector innerSelector)
            : base(innerSelector.Specificity, $"{PseudoClassNames.Separator}{PseudoClassNames.Not.StylesheetFunction(innerSelector.Text)}")
        {
            InnerSelector = innerSelector;
        }

        public ISelector InnerSelector { get; }
    }
}
