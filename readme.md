# ExCSS StyleSheet Parser for .NET

ExCSS is a CSS 2.1 and CSS 3 parser for .NET. Parses stylesheets into a queryable object model with full LINQ support.

## Basic Example
```cs
var parser = new StylesheetParser();
var stylesheet = parser.Parse(".someClass { color: red; background-image: url('/images/logo.png') }");

var rule = stylesheet.StyleRules.First() as StyleRule;
var selector = rule.SelectorText; // .someClass
var color = rule.Style.Color; // rgb(255, 0, 0)
var image = rule.Style.BackgroundImage; // url('/images/logo.png')
```

## Parser Options
```cs
var parser = new StylesheetParser(
    includeUnknownRules: false,        // keep unknown @-rules
    includeUnknownDeclarations: false, // keep unknown properties
    tolerateInvalidSelectors: false,   // don't fail on invalid selectors
    tolerateInvalidValues: true,       // keep values even if not recognized (default: true)
    tolerateInvalidConstraints: false, // lenient @media parsing
    preserveComments: false,           // keep CSS comments
    preserveDuplicateProperties: false // keep duplicate declarations
);
```

## Custom Pseudo-Class Registration
Register custom pseudo-classes for frameworks like Unity USS:
```cs
var parser = new StylesheetParser()
    .RegisterPseudoClass("unity-selected", () => PseudoClassSelector.Create("unity-selected"));

var stylesheet = parser.Parse(".item:unity-selected { color: blue; }");
var rule = stylesheet.StyleRules.First() as StyleRule;
var selector = rule.SelectorText; // .item:unity-selected
var color = rule.Style.Color; // rgb(0, 0, 255)
```

## Structured Selector Access
Nested selectors in `:not()`, `:has()`, `:matches()` are exposed as typed objects:
```cs
var selector = parser.ParseSelector(".btn:not(.disabled)");
foreach (var part in (CompoundSelector)selector)
{
    if (part is NotSelector not && not.InnerSelector is ClassSelector cs)
        Console.WriteLine($"Excluded: {cs.Class}"); // "disabled"
}
```

Available selector types: `ClassSelector`, `IdSelector`, `TypeSelector`, `PseudoClassSelector`, `PseudoElementSelector`, `NotSelector`, `HasSelector`, `MatchesSelector`, `CompoundSelector`, `ComplexSelector`.

## CSS Custom Properties
```cs
var stylesheet = parser.Parse(@"
:root { --primary: #007bff; }
.btn { color: var(--primary); }
");
var root = stylesheet.StyleRules.First() as StyleRule;
var primary = root.Style.GetPropertyValue("--primary"); // #007bff
```

## Supported Features
- **Properties**: `inset` shorthand, CSS Custom Properties (`var()`), all CSS3 properties
- **Selectors**: CSS Level 3 selectors, `:not()`, `:has()`, `:matches()`, `:nth-child()`, etc.
- **At-rules**: `@media`, `@keyframes`, `@font-face`, `@supports`, `@container`
- **AOT/Trimming**: Compatible with .NET Native AOT and IL trimming (Unity IL2CPP)

## Compatibility
- .NET 8.0, 7.0, 6.0, Core 3.1, Framework 4.8
- .NET Standard 2.0, 2.1
- Unity (via netstandard2.0)
