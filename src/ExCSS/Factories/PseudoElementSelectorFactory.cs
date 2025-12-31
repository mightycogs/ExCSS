using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    public sealed class PseudoElementSelectorFactory
    {
        private static readonly Lazy<PseudoElementSelectorFactory> Lazy =
            new(() => new PseudoElementSelectorFactory());

        private readonly StylesheetParser _parser;

        #region Standard Selectors

        private readonly Dictionary<string, ISelector> _selectors =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    PseudoElementNames.Before,
                    PseudoElementNames.After,
                    PseudoElementNames.Selection,
                    PseudoElementNames.FirstLine,
                    PseudoElementNames.FirstLetter,
                    PseudoElementNames.Content,
                }
                .ToDictionary(x => x, PseudoElementSelector.Create);

        #endregion

        #region Vendor Pseudo-Elements (valid extensions)

        private static readonly HashSet<string> VendorPseudoElements =
            new(StringComparer.OrdinalIgnoreCase)
            {
                // WebKit/Blink scrollbar
                "-webkit-scrollbar",
                "-webkit-scrollbar-button",
                "-webkit-scrollbar-thumb",
                "-webkit-scrollbar-track",
                "-webkit-scrollbar-track-piece",
                "-webkit-scrollbar-corner",
                "-webkit-resizer",
                // WebKit/Blink form controls
                "-webkit-input-placeholder",
                "-webkit-search-cancel-button",
                "-webkit-search-decoration",
                "-webkit-slider-runnable-track",
                "-webkit-slider-thumb",
                "-webkit-progress-bar",
                "-webkit-progress-value",
                "-webkit-meter-bar",
                "-webkit-inner-spin-button",
                "-webkit-outer-spin-button",
                "-webkit-file-upload-button",
                "-webkit-calendar-picker-indicator",
                "-webkit-details-marker",
                // Mozilla
                "-moz-placeholder",
                "-moz-focus-inner",
                "-moz-focus-outer",
                "-moz-list-bullet",
                "-moz-list-number",
                "-moz-progress-bar",
                "-moz-range-track",
                "-moz-range-thumb",
                "-moz-range-progress",
                "-moz-meter-bar",
                "-moz-color-swatch",
                // Microsoft
                "-ms-browse",
                "-ms-check",
                "-ms-clear",
                "-ms-expand",
                "-ms-fill",
                "-ms-fill-lower",
                "-ms-fill-upper",
                "-ms-reveal",
                "-ms-thumb",
                "-ms-track",
                "-ms-value",
                "-ms-input-placeholder",
            };

        #endregion

        internal PseudoElementSelectorFactory(StylesheetParser parser = null)
        {
            _parser = parser;
        }

        internal static PseudoElementSelectorFactory Instance => Lazy.Value;

        public ISelector Create(string name)
        {
            if (_selectors.TryGetValue(name, out var selector))
            {
                return selector;
            }

            if (VendorPseudoElements.Contains(name))
            {
                return PseudoElementSelector.Create(name);
            }

            if (_parser?.Options.AllowInvalidSelectors ?? false)
            {
                return PseudoElementSelector.Create(name);
            }

            return null;
        }
    }
}