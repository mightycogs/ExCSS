using System.Linq;
using Xunit;

namespace ExCSS.Tests.PropertyTests
{
    public class TypedValueTests
    {
        [Fact]
        public void TryGetValue_Color_ReturnsTrue()
        {
            var property = GetProperty("color: red");
            var result = property.TryGetValue<Color>(out var value);
            
            Assert.True(result);
            Assert.Equal(Color.Red, value);
        }

        [Fact]
        public void TryGetValue_Length_ReturnsTrue()
        {
            var property = GetProperty("width: 10px");
            var result = property.TryGetValue<Length>(out var value);
            
            Assert.True(result);
            Assert.Equal(10f, value.Value);
            Assert.Equal(Length.Unit.Px, value.Type);
        }

        [Fact]
        public void TryGetValue_WrongType_ReturnsFalse()
        {
            var property = GetProperty("width: 10px");
            var result = property.TryGetValue<Color>(out _);
            
            Assert.False(result);
        }

        [Fact]
        public void TryGetValue_Keyword_ReturnsTrue()
        {
            var property = GetProperty("display: block");
            var result = property.TryGetValue<KeywordValue>(out var value);
            
            Assert.True(result);
            Assert.Equal("block", value.Value);
        }

        private Property GetProperty(string css)
        {
            var parser = new StylesheetParser();
            var stylesheet = parser.Parse(".foo { " + css + " }");
            var rule = (StyleRule)stylesheet.Rules[0];
            return (Property)rule.Style.First();
        }
    }
}
