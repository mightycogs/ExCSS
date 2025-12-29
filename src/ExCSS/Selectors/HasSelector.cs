namespace ExCSS
{
    public sealed class HasSelector : SelectorBase
    {
        internal HasSelector(ISelector innerSelector)
            : base(innerSelector.Specificity, PseudoClassNames.Has.StylesheetFunction(innerSelector.Text))
        {
            InnerSelector = innerSelector;
        }

        public ISelector InnerSelector { get; }
    }
}
