using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    /// <summary>
    /// Converts grid-column/grid-row shorthand values to their longhand components.
    ///
    /// ARCHITECTURE NOTE (from Gemini consultation 2026-01):
    /// ExCSS has TWO expansion systems that are often confused:
    ///
    /// 1. PropertyFactory + ShorthandProperty (PARSING SYSTEM)
    ///    - Used during CSS parsing via StylesheetComposer
    ///    - ShorthandProperty.Export() calls DeclaredValue.ExtractFor(longhandName)
    ///    - The IPropertyValue returned by the converter MUST implement ExtractFor()
    ///
    /// 2. ShorthandRegistry (RUNTIME/COMPUTED SYSTEM)
    ///    - NOT used during parsing - only for runtime computed style expansion
    ///    - IShorthandExpander implementations are IGNORED by the parser
    ///
    /// COMMON MISTAKE: Creating an IShorthandExpander in ShorthandRegistry without
    /// a proper IValueConverter. The expander will never be called during parsing.
    ///
    /// CORRECT PATTERN (see MarginProperty/PeriodicValueConverter):
    /// 1. Create IValueConverter that returns IPropertyValue with ExtractFor()
    /// 2. ShorthandProperty uses this converter
    /// 3. ShorthandProperty.Export() extracts values via ExtractFor() for each longhand
    /// 4. Each longhand's TrySetValue() must accept the extracted TokenValue
    ///
    /// PITFALL: If longhand converters are too strict (e.g., IntegerConverter.Or(Identifier)),
    /// they will reject complex values like "span 2". Use Converters.Any for grid-line props.
    /// </summary>
    internal sealed class GridLineValueConverter : IValueConverter
    {
        private readonly string _startName;
        private readonly string _endName;

        public GridLineValueConverter(string startName, string endName)
        {
            _startName = startName;
            _endName = endName;
        }

        public IPropertyValue Convert(IEnumerable<Token> value)
        {
            var list = new List<Token>(value);
            if (list.Count == 0) return null;

            var slashIndex = list.FindIndex(t => t.Type == TokenType.Delim && t.Data == "/");

            IEnumerable<Token> startTokens;
            IEnumerable<Token> endTokens;

            if (slashIndex >= 0)
            {
                startTokens = list.Take(slashIndex);
                endTokens = list.Skip(slashIndex + 1);
            }
            else
            {
                startTokens = list;
                endTokens = Enumerable.Empty<Token>();
            }

            if (!startTokens.Any()) return null;

            return new GridLineValue(startTokens, endTokens, value, _startName, _endName);
        }

        public IPropertyValue Construct(Property[] properties)
        {
            var start = properties.FirstOrDefault(p => p.Name == _startName);
            var end = properties.FirstOrDefault(p => p.Name == _endName);

            if (start == null || end == null) return null;

            return new GridLineValue(start, end, _startName, _endName);
        }

        private sealed class GridLineValue : IPropertyValue
        {
            private readonly TokenValue _start;
            private readonly TokenValue _end;
            private readonly string _startName;
            private readonly string _endName;

            public GridLineValue(IEnumerable<Token> start, IEnumerable<Token> end, IEnumerable<Token> original, string startName, string endName)
            {
                _start = new TokenValue(start);

                if (end == null || !end.Any())
                {
                    _end = new TokenValue(new[] { new Token(TokenType.Ident, Keywords.Auto, TextPosition.Empty) });
                }
                else
                {
                    _end = new TokenValue(end);
                }

                Original = new TokenValue(original);
                _startName = startName;
                _endName = endName;
            }

            public GridLineValue(Property start, Property end, string startName, string endName)
            {
                _start = start.DeclaredValue != null ? start.DeclaredValue.Original : TokenValue.Initial;
                _end = end.DeclaredValue != null ? end.DeclaredValue.Original : TokenValue.Initial;
                _startName = startName;
                _endName = endName;
                Original = TokenValue.Initial;
            }

            public string CssText
            {
                get
                {
                    var startText = _start.ToText();
                    var endText = _end.ToText();

                    if (endText.Equals(Keywords.Auto, StringComparison.OrdinalIgnoreCase))
                    {
                        return startText;
                    }

                    return $"{startText} / {endText}";
                }
            }

            public TokenValue Original { get; }

            public TokenValue ExtractFor(string name)
            {
                if (name.Equals(_startName, StringComparison.OrdinalIgnoreCase)) return _start;
                if (name.Equals(_endName, StringComparison.OrdinalIgnoreCase)) return _end;
                return null;
            }
        }
    }
}
