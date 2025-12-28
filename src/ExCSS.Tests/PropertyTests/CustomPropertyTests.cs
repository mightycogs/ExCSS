using System.Linq;
using Xunit;

namespace ExCSS.Tests.PropertyTests;

public class CustomPropertyTests : CssConstructionFunctions
{
    [Fact]
    public void CustomPropertyDeclaration_SimpleName()
    {
        var snippet = "--primary-color: #007bff";
        var property = ParseDeclaration(snippet);
        Assert.Equal("--primary-color", property.Name);
        Assert.False(property.IsImportant);
        Assert.IsType<CustomProperty>(property);
        Assert.True(((Property)property).HasValue);
        Assert.Equal("#007bff", property.Value);
    }

    [Fact]
    public void CustomPropertyDeclaration_WithSpacing()
    {
        var snippet = "--spacing: 16px";
        var property = ParseDeclaration(snippet);
        Assert.Equal("--spacing", property.Name);
        Assert.IsType<CustomProperty>(property);
        Assert.Equal("16px", property.Value);
    }

    [Fact]
    public void CustomPropertyDeclaration_ComplexValue()
    {
        var snippet = "--border: 1px solid red";
        var property = ParseDeclaration(snippet);
        Assert.Equal("--border", property.Name);
        Assert.IsType<CustomProperty>(property);
        Assert.Equal("1px solid red", property.Value);
    }

    [Fact]
    public void CustomPropertyDeclaration_IsInherited()
    {
        var snippet = "--my-var: test";
        var property = ParseDeclaration(snippet);
        Assert.IsType<CustomProperty>(property);
        Assert.True(property.CanBeInherited);
    }

    [Fact]
    public void VarFunction_InPropertyValue()
    {
        var snippet = "color: var(--primary-color)";
        var property = ParseDeclaration(snippet, tolerateInvalidValues: true);
        Assert.Equal("color", property.Name);
        Assert.Equal("var(--primary-color)", property.Value);
    }

    [Fact]
    public void VarFunction_WithFallback()
    {
        var snippet = "padding: var(--spacing, 8px)";
        var property = ParseDeclaration(snippet, tolerateInvalidValues: true);
        Assert.Equal("padding", property.Name);
        Assert.Equal("var(--spacing, 8px)", property.Value);
    }

    [Fact]
    public void VarFunction_InCalc()
    {
        var snippet = "margin: calc(var(--spacing) * 2)";
        var property = ParseDeclaration(snippet, tolerateInvalidValues: true);
        Assert.Equal("margin", property.Name);
        Assert.Equal("calc(var(--spacing) * 2)", property.Value);
    }

    [Fact]
    public void FullStylesheet_WithCustomProperties()
    {
        var css = @":root {
    --primary-color: #007bff;
    --spacing: 16px;
}
.button {
    color: var(--primary-color);
    padding: var(--spacing, 8px);
    margin: calc(var(--spacing) * 2);
}";
        var stylesheet = ParseStyleSheet(css, tolerateInvalidValues: true);
        Assert.Equal(2, stylesheet.StyleRules.Count());

        var rootRule = stylesheet.StyleRules.First();
        Assert.Equal(":root", rootRule.SelectorText);
        var rootDecls = rootRule.Style.ToList();
        Assert.Equal(2, rootDecls.Count);
        Assert.Equal("--primary-color", rootDecls[0].Name);
        Assert.Equal("#007bff", rootDecls[0].Value);
        Assert.Equal("--spacing", rootDecls[1].Name);
        Assert.Equal("16px", rootDecls[1].Value);

        var buttonRule = stylesheet.StyleRules.Skip(1).First();
        Assert.Equal(".button", buttonRule.SelectorText);
        var buttonDecls = buttonRule.Style.ToList();
        Assert.Equal(3, buttonDecls.Count);
        Assert.Equal("color", buttonDecls[0].Name);
        Assert.Equal("var(--primary-color)", buttonDecls[0].Value);
        Assert.Equal("padding", buttonDecls[1].Name);
        Assert.Equal("var(--spacing, 8px)", buttonDecls[1].Value);
        Assert.Equal("margin", buttonDecls[2].Name);
        Assert.Equal("calc(var(--spacing) * 2)", buttonDecls[2].Value);
    }

    [Fact]
    public void CustomProperty_CaseSensitivity()
    {
        var css = @".test { --foo: red; --FOO: blue; --Foo: green; }";
        var sheet = ParseStyleSheet(css);
        var rule = sheet.StyleRules.First();

        Assert.Equal(3, rule.Style.Count());
        Assert.Equal("red", rule.Style.GetPropertyValue("--foo"));
        Assert.Equal("blue", rule.Style.GetPropertyValue("--FOO"));
        Assert.Equal("green", rule.Style.GetPropertyValue("--Foo"));
    }

    [Fact]
    public void CustomProperty_RejectsEmptyName()
    {
        var css = ".test { --: value; }";
        var sheet = ParseStyleSheet(css);
        var rule = sheet.StyleRules.First();

        Assert.Empty(rule.Style.Where(p => p is CustomProperty));
    }
}
