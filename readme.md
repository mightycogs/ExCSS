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

## Strongly-Typed Values
Access parsed values as typed objects instead of strings:
```cs
var stylesheet = parser.Parse(@"
.box {
    background-color: rgba(30, 30, 30, 0.95);
    width: calc(100% - 20px);
    color: var(--accent);
    opacity: 0.8;
}");
var rule = stylesheet.StyleRules.First() as StyleRule;

// Color with RGBA components
var bgColor = rule.Style.GetProperty("background-color").TypedValue as Color;
Console.WriteLine($"R:{bgColor.R} G:{bgColor.G} B:{bgColor.B}"); // R:30 G:30 B:30

// Calc expressions with full AST
var width = rule.Style.GetProperty("width").TypedValue as CalcValue;
var binary = width.Root as CalcBinaryExpression;
var left = (binary.Left as CalcLiteralExpression).Value as Percent;   // 100%
var right = (binary.Right as CalcLiteralExpression).Value as Length;  // 20px
Console.WriteLine($"{left.Value}% - {right.Value}{right.UnitString}"); // 100% - 20px

// CSS Variables
var color = rule.Style.GetProperty("color").TypedValue as VarValue;
Console.WriteLine(color.VariableName); // --accent

// Numeric values
var opacity = rule.Style.GetProperty("opacity").TypedValue as Number;
Console.WriteLine(opacity.Value); // 0.8
```

Available typed values: `Color`, `Length`, `Percent`, `Number`, `Time`, `CalcValue`, `VarValue`, `Gradient`, `Shadow`, and more.

Calc expression nodes: `CalcBinaryExpression` (+, -, *, /), `CalcLiteralExpression` (values), `CalcVarExpression` (var() refs), `CalcGroupExpression` (parentheses).

## Vendor-Prefixed Properties
Full support for vendor-prefixed CSS properties:
```cs
var stylesheet = parser.Parse(@"
.element {
    -webkit-appearance: none;
    -webkit-line-clamp: 3;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    user-select: none;
    pointer-events: auto;
}");
var rule = stylesheet.StyleRules.First() as StyleRule;

var appearance = rule.Style.GetPropertyValue("-webkit-appearance"); // none
var lineClamp = rule.Style.GetPropertyValue("-webkit-line-clamp"); // 3
```

Supported vendor properties: `appearance`, `-webkit-appearance`, `-webkit-line-clamp`, `-webkit-box-orient`, `-webkit-font-smoothing`, `-moz-osx-font-smoothing`, `user-select`, `overflow-x`, `overflow-y`, `pointer-events`.

## Supported Features
- **Properties**: `inset` shorthand, CSS Custom Properties (`var()`), vendor prefixes, all CSS3 properties
- **Values**: Strongly-typed access to colors, lengths, calc(), var(), gradients, shadows
- **Selectors**: CSS Level 3 selectors, `:not()`, `:has()`, `:matches()`, `:nth-child()`, vendor pseudo-elements
- **At-rules**: `@media`, `@keyframes`, `@font-face`, `@supports`, `@container`
- **AOT/Trimming**: Compatible with .NET Native AOT and IL trimming (Unity IL2CPP)
- **Resilience**: Graceful recovery from malformed CSS without crashing

## Compatibility
- .NET 8.0, 7.0, 6.0, Core 3.1, Framework 4.8
- .NET Standard 2.0, 2.1
- Unity (via netstandard2.0)
