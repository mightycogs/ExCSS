using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    public sealed class PseudoClassSelectorFactory
    {
        private static readonly Lazy<PseudoClassSelectorFactory> Lazy =
            new(() =>
                {
                    var factory = new PseudoClassSelectorFactory();
                    DefaultSelectors.Add(PseudoElementNames.Before,
                        PseudoElementSelectorFactory.Instance.Create(PseudoElementNames.Before));
                    DefaultSelectors.Add(PseudoElementNames.After,
                        PseudoElementSelectorFactory.Instance.Create(PseudoElementNames.After));
                    DefaultSelectors.Add(PseudoElementNames.FirstLine,
                        PseudoElementSelectorFactory.Instance.Create(PseudoElementNames.FirstLine));
                    DefaultSelectors.Add(PseudoElementNames.FirstLetter,
                        PseudoElementSelectorFactory.Instance.Create(PseudoElementNames.FirstLetter));
                    return factory;
                }
            );

        #region Selectors

        private static readonly Dictionary<string, ISelector> DefaultSelectors =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    PseudoClassNames.Root,
                    PseudoClassNames.Scope,
                    PseudoClassNames.OnlyType,
                    PseudoClassNames.FirstOfType,
                    PseudoClassNames.LastOfType,
                    PseudoClassNames.OnlyChild,
                    PseudoClassNames.FirstChild,
                    PseudoClassNames.LastChild,
                    PseudoClassNames.Empty,
                    PseudoClassNames.AnyLink,
                    PseudoClassNames.Link,
                    PseudoClassNames.Visited,
                    PseudoClassNames.Active,
                    PseudoClassNames.Hover,
                    PseudoClassNames.Focus,
                    PseudoClassNames.FocusVisible,
                    PseudoClassNames.FocusWithin,
                    PseudoClassNames.Target,
                    PseudoClassNames.Enabled,
                    PseudoClassNames.Disabled,
                    PseudoClassNames.Default,
                    PseudoClassNames.Checked,
                    PseudoClassNames.Indeterminate,
                    PseudoClassNames.PlaceholderShown,
                    PseudoClassNames.Unchecked,
                    PseudoClassNames.Valid,
                    PseudoClassNames.Invalid,
                    PseudoClassNames.Required,
                    PseudoClassNames.ReadOnly,
                    PseudoClassNames.ReadWrite,
                    PseudoClassNames.InRange,
                    PseudoClassNames.OutOfRange,
                    PseudoClassNames.Optional,
                    PseudoClassNames.Shadow,
                }
                .ToDictionary(x => x, PseudoClassSelector.Create);

        #endregion

        private readonly ConcurrentDictionary<string, Func<ISelector>> _customSelectors =
            new(StringComparer.OrdinalIgnoreCase);

        internal static PseudoClassSelectorFactory Instance => Lazy.Value;

        internal void Register(string name, Func<ISelector> factory)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            _customSelectors[name] = factory;
        }

        public ISelector Create(string name)
        {
            if (_customSelectors.TryGetValue(name, out var factory))
            {
                return factory();
            }
            return DefaultSelectors.TryGetValue(name, out var selector) ? selector : null;
        }

        internal PseudoClassSelectorFactory Clone()
        {
            var clone = new PseudoClassSelectorFactory();
            foreach (var kvp in _customSelectors)
            {
                clone._customSelectors[kvp.Key] = kvp.Value;
            }
            return clone;
        }
    }
}