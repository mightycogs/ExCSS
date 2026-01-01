# Shorthand Expanders - Next Targets

Last updated: 2026-01-02

Recommended order (quick wins first):

1. `list-style` → `list-style-type`, `list-style-position`, `list-style-image` (low complexity)
2. `outline` → `outline-color`, `outline-style`, `outline-width` (low complexity; mirrors `border`)
3. `place-content`, `place-items`, `place-self` (two-axis aliases; low complexity)
4. `text-decoration` → `text-decoration-line`, `text-decoration-color`, `text-decoration-style`, `text-decoration-thickness` (medium complexity)
5. Logical borders/margins/paddings:
   - `border-top`, `border-right`, `border-bottom`, `border-left` (style/width/color)
   - `border-inline`, `border-block`, `margin-inline`, `margin-block`, `padding-inline`, `padding-block` (medium complexity; RTL-friendly)
6. `columns` / `column-rule` / `column-gap` → `column-width`, `column-count`, `column-rule-color`, `column-rule-style`, `column-rule-width` (medium complexity)
7. `animation` → name, duration, timing-function, delay, iteration-count, direction, fill-mode, play-state (high complexity)
8. `transition` → property, duration, timing-function, delay (medium-high complexity)
9. `font` → style, variant, weight, stretch, size, line-height, family (high complexity; ordering rules)

Notes:
- Keep parity with existing border handling; reuse `BorderExpander` patterns where possible.
- Prioritize test coverage for edge cases: multiple values, global keywords, logical properties, and mixed units.
