# CSS Extended Tests Plan - 2025-12-31

## Cel
Upewnić się, że parser ExCSS rozumie **całą przestrzeń definicji CSS** używaną w MightyUI. Każdy fragment musi być:
1. Parsowany bez błędów
2. Semantycznie zrozumiany (strongly-typed, zero `RawValue` dla znanych właściwości)

## Źródła CSS
- `/Users/developer/Projects3/mighty-ui/docs/ui/client/src/App.css` (1761 linii)
- `/Users/developer/Projects3/mighty-ui/docs/ui/client/src/index.css` (13 linii)

## Fazy

| Faza | Zakres | Status |
|------|--------|--------|
| **Phase 1** | Happy path - fragmenty CSS do osobnych plików, testy strongly-typed | TODO |
| Phase 2 | Snapshot testing dla AST selektorów | Nice-to-have |
| Phase 3 | Location info (line/column) w błędach | Nice-to-have |
| Phase 4 | Negative tests - niepoprawna składnia | Nice-to-have |

---

## Phase 1: Happy Path

### Struktura katalogów

```
src/ExCSS.Tests/
└── ExtendedTestsPart1/
    ├── Fixtures/
    │   ├── Selectors/
    │   │   ├── 001_root_html_body_id.css
    │   │   ├── 002_adjacent_sibling_universal.css
    │   │   ├── 003_hover_multiple_not.css
    │   │   ├── 004_multiple_selectors_descendant.css
    │   │   ├── 005_nth_child.css
    │   │   ├── 006_first_last_child.css
    │   │   ├── 007_webkit_pseudo_element.css
    │   │   ├── 008_pseudo_class_after.css
    │   │   └── ...
    │   ├── Values/
    │   │   ├── 001_css_variable.css
    │   │   ├── 002_rgba_color.css
    │   │   ├── 003_hex_color.css
    │   │   ├── 004_calc_simple.css
    │   │   ├── 005_calc_with_var.css
    │   │   ├── 006_box_shadow_multi.css
    │   │   ├── 007_radial_gradient.css
    │   │   ├── 008_linear_gradient.css
    │   │   ├── 009_transition_shorthand.css
    │   │   ├── 010_font_stack.css
    │   │   └── ...
    │   ├── Properties/
    │   │   ├── 001_custom_property.css
    │   │   ├── 002_inset_shorthand.css
    │   │   ├── 003_border_shorthand.css
    │   │   ├── 004_backdrop_filter.css
    │   │   ├── 005_vendor_prefix.css
    │   │   └── ...
    │   └── AtRules/
    │       ├── 001_keyframes.css
    │       └── ...
    ├── SelectorTests.cs
    ├── ValueTests.cs
    ├── PropertyTests.cs
    └── AtRuleTests.cs
```

### Fragmenty CSS do wyekstrahowania

#### Selectors

| # | Plik | Fragment | Testuje |
|---|------|----------|---------|
| 001 | root_html_body_id.css | `:root, html, body, #root { height: 100%; }` | Selector list, pseudo-class, element, ID |
| 002 | adjacent_sibling_universal.css | `.designer-container > * + * { margin-left: var(--spacing-lg); }` | Child combinator, adjacent sibling, universal |
| 003 | hover_multiple_not.css | `.part-card:hover:not(.locked):not(.selected) .part-thumbnail { box-shadow: inset 1px 0 0 0 var(--accent-green), inset 0 1px 0 0 var(--accent-green), inset 0 -1px 0 0 var(--accent-green); }` | Multiple :not(), descendant |
| 004 | multiple_selectors_descendant.css | `.part-card.selected .part-name, .part-card.selected .part-price { color: #000; }` | Multiple selectors, compound, descendant |
| 005 | nth_child.css | `.settings-section .setting-row:nth-child(odd) { background: rgba(0, 0, 0, 0.2); }` | :nth-child() |
| 006 | first_last_child.css | `.toggle-btn:first-child { border-radius: var(--radius-sm) 0 0 var(--radius-sm); }` | :first-child |
| 007 | webkit_pseudo_element.css | `::-webkit-scrollbar { width: 6px; }` | Vendor pseudo-element |
| 008 | pseudo_after_content.css | `.part-card.selected::after { content: ''; position: absolute; ... }` | ::after, content |

#### Values

| # | Plik | Fragment | Testuje |
|---|------|----------|---------|
| 001 | css_variable.css | `.stat-item { color: var(--accent-green); }` | VarValue |
| 002 | rgba_color.css | `.panel-bg { background: rgba(30, 30, 30, 0.95); }` | Color (rgba) |
| 003 | hex_color.css | `.bg-dark { background: #1a1a1a; }` | Color (hex) |
| 004 | calc_simple.css | `.app-container { width: calc(100% - 64px); }` | CalcValue |
| 005 | calc_with_var_negative.css | `.part-card.selected::after { right: calc(-1 * var(--card-notch-size)); }` | CalcValue nested |
| 006 | box_shadow_multi.css | (z 003 selectors) | StyleValueList<Shadow> |
| 007 | radial_gradient.css | `.part-thumbnail { background: radial-gradient(ellipse 100% 80% at center, rgba(255, 255, 255, 0.15) 0%, transparent 70%), #000; }` | Gradient |
| 008 | linear_gradient.css | `.title-overlay { background: linear-gradient(to right, rgba(0, 0, 0, 0.9) 0%, transparent 100%); }` | Gradient |
| 009 | transition_shorthand.css | `.menu-btn { transition: all var(--transition-fast); }` | Time + keyword |
| 010 | font_stack.css | `body { font-family: 'Segoe UI', -apple-system, sans-serif; }` | Font list |
| 011 | time_ease.css | `:root { --transition-fast: 0.15s ease; }` | Time + identifier |
| 012 | unitless_number.css | `.part-name { line-height: 1.2; }` | Number |

#### Properties

| # | Plik | Fragment | Testuje |
|---|------|----------|---------|
| 001 | custom_property.css | `:root { --bg-dark: #1a1a1a; }` | Custom property definition |
| 002 | inset_shorthand.css | `.card-lock-overlay { inset: 0; }` | Inset shorthand |
| 003 | border_shorthand.css | `.menu-btn { border: 1px solid var(--border-color); }` | Border shorthand |
| 004 | backdrop_filter.css | `.main-menu { backdrop-filter: blur(10px); }` | Filter function |
| 005 | vendor_webkit.css | `.main-menu { -webkit-backdrop-filter: blur(10px); }` | Vendor prefix |
| 006 | vendor_moz.css | `.cheat-input { -moz-appearance: textfield; }` | Vendor prefix |

#### At-Rules

| # | Plik | Fragment | Testuje |
|---|------|----------|---------|
| 001 | keyframes.css | `@keyframes kenburns { 0% { transform: scale(1); } 100% { transform: scale(1.05); } }` | KeyframeRule |

### Asercje dla każdego testu

```csharp
// Pattern dla testu selektora
[Theory]
[InlineData("Fixtures/Selectors/001_root_html_body_id.css")]
public void Selector_RootHtmlBodyId_ParsesCorrectly(string fixturePath)
{
    var css = File.ReadAllText(fixturePath);
    var sheet = _parser.Parse(css);

    sheet.StyleRules.Should().HaveCount(1);
    var rule = sheet.StyleRules.First();

    // Selector list: 4 selectors
    rule.Selector.Should().BeOfType<SelectorList>();
    var list = (SelectorList)rule.Selector;
    list.Should().HaveCount(4);

    // First: :root (pseudo-class)
    list[0].Should().BeOfType<PseudoClassSelector>();

    // Declarations strongly-typed
    var height = rule.Style["height"];
    height.TypedValue.Should().BeOfType<Percent>();
    ((Percent)height.TypedValue).Value.Should().Be(100);
}
```

### Kryteria sukcesu Phase 1

1. **100% fixtures parsuje się bez błędów**
2. **Zero `RawValue` dla znanych właściwości** (height, color, margin, padding, etc.)
3. **Selektory parsują się do drzewa** (nie string)
4. **Wartości mają konkretne typy:**
   - `var(...)` → `VarValue`
   - `#hex` / `rgba(...)` → `Color`
   - `100%` → `Percent`
   - `64px` → `Length`
   - `calc(...)` → `CalcValue`
   - `1.2` → `Number`
   - box-shadow → `Shadow` lub `StyleValueList<Shadow>`

---

## Implementacja

### Kolejność kroków

1. Utworzyć strukturę katalogów `ExtendedTestsPart1/Fixtures/`
2. Wyekstrahować fragmenty CSS do plików fixture (zachowując whitespace)
3. Napisać klasę bazową `ExtendedTestBase` z helper metodami
4. Implementować testy TDD: red → green
5. Jeśli parser nie obsługuje składni → rozszerzyć parser

### Estymacja

- ~30 fixtures do wyekstrahowania
- ~4 pliki testowe
- ~60-80 asercji
