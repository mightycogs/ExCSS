# Shorthand Expanders - Next Targets

Last updated: 2026-01-02

## Phase Plan

### Phase 1 — Quick wins
1. `list-style` → `list-style-type`, `list-style-position`, `list-style-image`
2. `outline` → `outline-color`, `outline-style`, `outline-width`
3. `place-content`, `place-items`, `place-self`

### Phase 2 — Medium set
4. `text-decoration` → `text-decoration-line`, `text-decoration-color`, `text-decoration-style`, `text-decoration-thickness`
5. Logical borders/margins/paddings:
   - `border-top`, `border-right`, `border-bottom`, `border-left`
   - `border-inline`, `border-block`, `margin-inline`, `margin-block`, `padding-inline`, `padding-block`
6. `columns` / `column-rule` / `column-gap` → `column-width`, `column-count`, `column-rule-color`, `column-rule-style`, `column-rule-width`

### Phase 3 — Complex shorthands
7. `animation` → name, duration, timing-function, delay, iteration-count, direction, fill-mode, play-state
8. `transition` → property, duration, timing-function, delay
9. `font` → style, variant, weight, stretch, size, line-height, family

Notes:
- Keep parity with existing border handling; reuse `BorderExpander` patterns where possible.
- Prioritize test coverage for edge cases: multiple values, global keywords, logical properties, and mixed units.
