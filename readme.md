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

Available typed values: `Color`, `Length`, `Percent`, `Number`, `Time`, `Angle`, `AspectRatio`, `CalcValue`, `VarValue`, `Gradient`, `Shadow`, `FunctionValue` (for filters), and more.

Calc expression nodes: `CalcBinaryExpression` (+, -, *, /), `CalcLiteralExpression` (values), `CalcVarExpression` (var() refs), `CalcGroupExpression` (parentheses).

## Gradient Typed Values
Access gradient properties through `LinearGradient` and `RadialGradient`:
```cs
var stylesheet = parser.Parse(".box { background: linear-gradient(45deg, red 0%, blue 100%); }");
var rule = stylesheet.StyleRules.First() as StyleRule;
var prop = rule.Style.GetProperty("background");

if (prop.TypedValue is LinearGradient lg)
{
    Console.WriteLine($"Angle: {lg.Angle}");           // 45deg
    Console.WriteLine($"Repeating: {lg.IsRepeating}"); // false
    foreach (var stop in lg.Stops)
        Console.WriteLine($"  {stop.Color} at {stop.Location}");
}
```

Radial gradients expose shape, position, and size:
```cs
var stylesheet = parser.Parse(".box { background: radial-gradient(circle at center, red, blue); }");
// ...
if (prop.TypedValue is RadialGradient rg)
{
    Console.WriteLine($"IsCircle: {rg.IsCircle}");     // true
    Console.WriteLine($"Position: {rg.Position}");    // 50% 50%
    Console.WriteLine($"SizeMode: {rg.SizeMode}");    // FarthestCorner
}
```

## Modern Layout & Effects
Support for modern layout and visual effects:
```cs
var stylesheet = parser.Parse(@"
.card {
    aspect-ratio: 16 / 9;
    backdrop-filter: blur(10px) saturate(180%);
}");
var rule = stylesheet.StyleRules.First() as StyleRule;

// Aspect Ratio (returns typed AspectRatio)
var ratio = rule.Style.GetProperty("aspect-ratio").TypedValue as AspectRatio;
Console.WriteLine($"{ratio.Width} / {ratio.Height}"); // 16 / 9
Console.WriteLine($"Ratio: {ratio.Value}");           // 1.777...

// Backdrop Filter (single function or list)
var filterProp = rule.Style.GetProperty("backdrop-filter");
if (filterProp.TypedValue is FunctionValue single)
{
    Console.WriteLine($"{single.Name}({single.Arguments})"); // blur(10px)
}
else if (filterProp.TypedValue is StyleValueList list)
{
    foreach (var fn in list.OfType<FunctionValue>())
        Console.WriteLine($"{fn.Name}({fn.Arguments})"); // blur(10px), saturate(180%)
}
```

## Shadow Parsing Helper
Convert box-shadow values to typed `Shadow` objects:
```cs
var stylesheet = parser.Parse(".card { box-shadow: inset 2px 3px 4px 5px red; }");
var rule = stylesheet.StyleRules.First() as StyleRule;
var prop = rule.Style.GetProperty("box-shadow");

var shadow = Shadow.TryParse(prop.TypedValue, prop.Value);
Console.WriteLine($"Inset: {shadow.IsInset}");       // true
Console.WriteLine($"OffsetX: {shadow.OffsetX}");    // 2px
Console.WriteLine($"OffsetY: {shadow.OffsetY}");    // 3px
Console.WriteLine($"Blur: {shadow.BlurRadius}");    // 4px
Console.WriteLine($"Spread: {shadow.SpreadRadius}"); // 5px
Console.WriteLine($"Color: {shadow.Color}");        // rgb(255, 0, 0)
```

For shadows without inset, the `cssText` parameter is optional:
```cs
var shadow = Shadow.TryParse(prop.TypedValue); // works for non-inset shadows
```

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

## Shorthand Expansion
Shorthand properties are automatically expanded to longhands during parsing. Access typed values directly from expanded properties:

```cs
var style = parser.Parse(".box { margin: 10px auto; flex: 2 1 100px; }").StyleRules.First().Style;

// Margin expands to 4 longhands
var top = style.Declarations.First(d => d.Name == "margin-top").TypedValue as Length;
var right = style.Declarations.First(d => d.Name == "margin-right").TypedValue as KeywordValue; // auto

// Flex expands to 3 longhands
var grow = style.Declarations.First(d => d.Name == "flex-grow").TypedValue as Number;
var basis = style.Declarations.First(d => d.Name == "flex-basis").TypedValue as Length;
Console.WriteLine($"grow: {grow.Value}, basis: {basis.Value}px"); // grow: 2, basis: 100px
```

| Shorthand | Longhands | Typed Values |
|-----------|-----------|--------------|
| `margin`, `padding` | `*-top/right/bottom/left` | `Length`, `Percent`, `KeywordValue` (auto) |
| `border` | `border-*-width/style/color` | `Length`, `KeywordValue`, `Color` |
| `flex` | `flex-grow`, `flex-shrink`, `flex-basis` | `Number`, `Length`, `Percent` |
| `gap` | `row-gap`, `column-gap` | `Length`, `Percent` |
| `background` | 8 longhands including `background-color` | `Color`, `UrlValue`, `KeywordValue` |

For manual expansion use `ShorthandRegistry.Expand(name, value)`.

Supported shorthands: `margin`, `padding`, `inset`, `border`, `border-radius`, `background`, `flex`, `flex-flow`, `gap`, `animation`, `transition`, `font`, `text-decoration`, `outline`, `list-style`, `columns`, `column-rule`, `place-content`, `place-items`, `place-self`, `border-top/right/bottom/left`, `border-inline`, `border-block`, `margin-inline`, `margin-block`, `padding-inline`, `padding-block`.

## Supported Features
- **Shorthand Expansion**: 25+ CSS shorthands → longhands (background 8-way, animation 8-way, font 7-way, etc.)
- **Typed Values**: Colors, lengths, calc(), var(), gradients, shadows, aspect-ratio, filter functions as strongly-typed objects
- **CSS Custom Properties**: Full `var()` support with fallbacks
- **Vendor Prefixes**: `-webkit-*`, `-moz-*` properties parsed correctly
- **Selectors**: CSS Level 3 selectors, `:not()`, `:has()`, `:matches()`, `:nth-child()`
- **At-rules**: `@media`, `@keyframes`, `@font-face`, `@supports`, `@container`
- **AOT/Trimming**: Compatible with .NET Native AOT and IL trimming (Unity IL2CPP)

## Compatibility
- .NET 8.0, 7.0, 6.0, Core 3.1, Framework 4.8
- .NET Standard 2.0, 2.1
- Unity (via netstandard2.0)
