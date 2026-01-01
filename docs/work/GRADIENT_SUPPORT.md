# ExCSS Gradient Support Analysis

**Date:** 2026-01-01
**Requested by:** mighty-ui team
**Status:** Analysis Complete

## Problem Statement

In mighty-ui, gradients cannot be accessed through the typed values pipeline (`TryGetValue<T>`). The `LinearGradient` and `RadialGradient` types exist but do not implement `IStyleValue`, requiring fallback to string parsing.

---

## Current Status

### What Exists

#### 1. Gradient Data Types (DO NOT implement IStyleValue)

**File:** `/Users/developer/Projects3/ExCSS/src/ExCSS/Values/LinearGradient.cs` (lines 1-21)
```csharp
public sealed class LinearGradient : IGradient
{
    public Angle Angle { get; }
    public IEnumerable<GradientStop> Stops { get; }
    public bool IsRepeating { get; }
}
```

**File:** `/Users/developer/Projects3/ExCSS/src/ExCSS/Values/RadialGradient.cs` (lines 1-40)
```csharp
public sealed class RadialGradient : IImageSource  // Note: implements IImageSource, not IGradient!
{
    public bool IsCircle { get; }
    public SizeMode Mode { get; }
    public Point Position { get; }
    public Length MajorRadius { get; }
    public Length MinorRadius { get; }
    public IEnumerable<GradientStop> Stops { get; }
    public bool IsRepeating { get; }
}
```

**Observation:** `LinearGradient` implements `IGradient`, but `RadialGradient` only implements `IImageSource` directly. This appears to be an inconsistency.

#### 2. IGradient Interface

**File:** `/Users/developer/Projects3/ExCSS/src/ExCSS/Values/IGradient.cs` (lines 1-10)
```csharp
public interface IGradient : IImageSource
{
    IEnumerable<GradientStop> Stops { get; }
    bool IsRepeating { get; }
}
```

#### 3. IImageSource Interface (Empty marker)

**File:** `/Users/developer/Projects3/ExCSS/src/ExCSS/Values/IImageSource.cs` (lines 1-6)
```csharp
public interface IImageSource
{
    // Empty marker interface
}
```

#### 4. GradientFunctionValue (DOES implement IStyleValue)

**File:** `/Users/developer/Projects3/ExCSS/src/ExCSS/Values/GradientFunctionValue.cs` (lines 1-22)
```csharp
public sealed class GradientFunctionValue : IStyleValue, IFunctionValue
{
    public string Name { get; }      // "linear-gradient", "radial-gradient", etc.
    public string Arguments { get; } // Raw argument string
    public bool IsRepeating { get; }

    public string CssText => $"{Name}({Arguments})";
    public StyleValueType Type => StyleValueType.Gradient;
}
```

#### 5. How Gradients Flow Through the System

The parsing pipeline works as follows:

1. **FunctionValueConverter** (`/Users/developer/Projects3/ExCSS/src/ExCSS/ValueConverters/FunctionValueConverter.cs`, lines 58-71):
   - Detects gradient functions
   - Returns `GradientFunctionValue` via `ITypedPropertyValue.GetValue()`

2. **AnyValueConverter** (`/Users/developer/Projects3/ExCSS/src/ExCSS/ValueConverters/AnyValueConverter.cs`, lines 29-34, 119-131):
   - Detects gradient functions
   - Creates `GradientFunctionValue` with raw argument string
   - Wraps in `TypedAnyValue` which implements `ITypedPropertyValue`

3. **GradientConverter** (`/Users/developer/Projects3/ExCSS/src/ExCSS/ValueConverters/GradientConverter.cs`):
   - Exists but returns internal `GradientValue` (IPropertyValue)
   - Does NOT return the public `LinearGradient`/`RadialGradient` types

---

## Verdict: Missing Feature (Incomplete Implementation)

**Classification:** Missing feature / incomplete typed values pipeline

The ExCSS library has **two parallel systems** for gradients:

| System | Types | Implements IStyleValue | Used By |
|--------|-------|----------------------|---------|
| **Legacy** | `LinearGradient`, `RadialGradient` | NO | Nothing currently |
| **Typed Pipeline** | `GradientFunctionValue` | YES | `TryGetValue<T>()`, typed API |

The `LinearGradient` and `RadialGradient` classes appear to be **legacy types** that predate the typed values system. When the `IStyleValue` interface was added (per PLAN_20251231.md), gradients were handled by creating `GradientFunctionValue` instead of retrofitting the legacy types.

**Why `GradientFunctionValue` is a workaround, not a solution:**

1. It stores arguments as a raw string, not parsed data
2. Consumers must re-parse the gradient arguments themselves
3. The rich type information in `LinearGradient`/`RadialGradient` is inaccessible

---

## Change Request

### Option A: Full Implementation (Recommended)

Make `LinearGradient` and `RadialGradient` implement `IStyleValue` and integrate them into the typed pipeline.

#### Files to Modify

1. **`/Users/developer/Projects3/ExCSS/src/ExCSS/Values/LinearGradient.cs`**

```csharp
public sealed class LinearGradient : IGradient, IStyleValue
{
    // Existing code...

    // ADD: IStyleValue implementation
    public string CssText
    {
        get
        {
            var prefix = IsRepeating ? "repeating-linear-gradient" : "linear-gradient";
            var args = new List<string>();

            // Add angle if not default (180deg / to bottom)
            if (Angle.Value != 180f || Angle.Type != Angle.Unit.Deg)
                args.Add(Angle.CssText);

            // Add stops
            foreach (var stop in Stops)
                args.Add($"{stop.Color.CssText} {stop.Location.CssText}".Trim());

            return $"{prefix}({string.Join(", ", args)})";
        }
    }

    public StyleValueType Type => StyleValueType.Gradient;
}
```

2. **`/Users/developer/Projects3/ExCSS/src/ExCSS/Values/RadialGradient.cs`**

```csharp
public sealed class RadialGradient : IGradient, IStyleValue  // Note: Change IImageSource to IGradient
{
    // Existing code...

    // ADD: IStyleValue implementation
    public string CssText
    {
        get
        {
            var prefix = IsRepeating ? "repeating-radial-gradient" : "radial-gradient";
            var args = new List<string>();

            // Add shape/size configuration
            // ... (complex logic for circle/ellipse/size-mode/position)

            // Add stops
            foreach (var stop in Stops)
                args.Add($"{stop.Color.CssText} {stop.Location.CssText}".Trim());

            return $"{prefix}({string.Join(", ", args)})";
        }
    }

    public StyleValueType Type => StyleValueType.Gradient;
}
```

3. **`/Users/developer/Projects3/ExCSS/src/ExCSS/Values/GradientStop.cs`**

```csharp
public struct GradientStop : IStyleValue
{
    // Existing code...

    // ADD: IStyleValue implementation
    public string CssText => Location.Value == 0
        ? Color.CssText
        : $"{Color.CssText} {Location.CssText}";

    public StyleValueType Type => StyleValueType.Unknown; // Or add StyleValueType.GradientStop
}
```

4. **`/Users/developer/Projects3/ExCSS/src/ExCSS/ValueConverters/GradientConverter.cs`**

Modify `GradientConverter` to produce `LinearGradient`/`RadialGradient` objects through `ITypedPropertyValue.GetValue()`.

The current `GradientValue` private class (line 94-131) needs to implement `ITypedPropertyValue` and return the appropriate gradient type:

```csharp
// Line ~99: Add ITypedPropertyValue
private sealed class GradientValue : IPropertyValue, ITypedPropertyValue
{
    // ... existing code ...

    public object GetValue()
    {
        // Parse stops
        var gradientStops = _stops.Select(s => ParseStop(s)).ToArray();

        // Parse initial argument to determine type
        if (/* is linear */)
            return new LinearGradient(angle, gradientStops, isRepeating);
        else
            return new RadialGradient(/* params */, gradientStops, isRepeating);
    }
}
```

5. **`/Users/developer/Projects3/ExCSS/src/ExCSS/ValueConverters/AnyValueConverter.cs`**

Update `ExtractGradientValue` (line 127-131) to parse and return full `LinearGradient`/`RadialGradient` objects instead of `GradientFunctionValue`.

#### Effort Estimate

- **Low-risk changes:** `LinearGradient.cs`, `RadialGradient.cs`, `GradientStop.cs` (add interface, add CssText)
- **Medium-risk changes:** `GradientConverter.cs` (add ITypedPropertyValue, implement GetValue)
- **Higher-risk changes:** `AnyValueConverter.cs` (parse gradient arguments into typed objects)

**Estimated effort:** 2-4 hours

---

### Option B: Keep GradientFunctionValue, Add Parsing Helper (Quick Fix)

If full implementation is too risky, add a helper method to parse `GradientFunctionValue` into the legacy types.

**File:** `/Users/developer/Projects3/ExCSS/src/ExCSS/Values/GradientFunctionValue.cs`

```csharp
public sealed class GradientFunctionValue : IStyleValue, IFunctionValue
{
    // Existing code...

    /// <summary>
    /// Parse this gradient into a strongly-typed LinearGradient or RadialGradient.
    /// </summary>
    public IGradient Parse()
    {
        if (Name.Contains("linear"))
            return ParseLinearGradient();
        if (Name.Contains("radial"))
            return ParseRadialGradient();
        return null;
    }

    private LinearGradient ParseLinearGradient() { /* ... */ }
    private RadialGradient ParseRadialGradient() { /* ... */ }
}
```

**Effort estimate:** 1-2 hours

---

### Option C: Extension Method in mighty-ui (Workaround)

Create an extension method in mighty-ui that parses `GradientFunctionValue.Arguments`:

```csharp
// In mighty-ui
public static class GradientExtensions
{
    public static LinearGradient ToLinearGradient(this GradientFunctionValue gfv) { /* parse */ }
    public static RadialGradient ToRadialGradient(this GradientFunctionValue gfv) { /* parse */ }
}
```

**Note:** This keeps the workaround in mighty-ui and doesn't fix ExCSS.

---

## Existing Tests

Tests exist for gradient parsing but do not verify typed value extraction:

**File:** `/Users/developer/Projects3/ExCSS/src/ExCSS.Tests/ExtendedTestsPart1/ValueTests.cs`
- Lines 104-115: `Parse_RadialGradient_GradientValue` - Only checks `!RawValue`
- Lines 117-128: `Parse_LinearGradient_GradientValue` - Only checks `!RawValue`

**File:** `/Users/developer/Projects3/ExCSS/src/ExCSS.Tests/Gradient.cs`
- Tests parsing/serialization but not typed values

---

## Summary

| Item | Status |
|------|--------|
| `LinearGradient` class exists | Yes |
| `RadialGradient` class exists | Yes |
| `LinearGradient` implements `IStyleValue` | **No** |
| `RadialGradient` implements `IStyleValue` | **No** |
| `RadialGradient` implements `IGradient` | **No** (inconsistency) |
| Gradients work through typed pipeline | Partial (via `GradientFunctionValue`) |
| Typed gradient data accessible | **No** (arguments stored as string) |

**Recommendation:** Implement Option A for full typed support. The legacy `LinearGradient`/`RadialGradient` types have rich data models that should be exposed through the typed pipeline.
