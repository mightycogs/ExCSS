using System.Linq;
using Xunit;

namespace ExCSS.Tests.ExtendedTestsPart1
{
    public class AtRuleTests : ExtendedTestBase
    {
        [Fact]
        public void Parse_Keyframes_HasKeyframesRule()
        {
            var sheet = ParseFixture("AtRules", "001_keyframes.css");

            Assert.NotEmpty(sheet.Rules);
            var keyframesRule = sheet.Rules.OfType<KeyframesRule>().FirstOrDefault();
            Assert.NotNull(keyframesRule);
        }

        [Fact]
        public void Parse_Keyframes_CorrectName()
        {
            var sheet = ParseFixture("AtRules", "001_keyframes.css");
            var keyframesRule = sheet.Rules.OfType<KeyframesRule>().FirstOrDefault();

            Assert.NotNull(keyframesRule);
            Assert.Equal("kenburns", keyframesRule.Name);
        }

        [Fact]
        public void Parse_Keyframes_HasTwoKeyframes()
        {
            var sheet = ParseFixture("AtRules", "001_keyframes.css");
            var keyframesRule = sheet.Rules.OfType<KeyframesRule>().FirstOrDefault();

            Assert.NotNull(keyframesRule);
            Assert.Equal(2, keyframesRule.Rules.Count());
        }

        [Fact]
        public void Parse_Keyframes_HasZeroPercent()
        {
            var sheet = ParseFixture("AtRules", "001_keyframes.css");
            var keyframesRule = sheet.Rules.OfType<KeyframesRule>().FirstOrDefault();

            Assert.NotNull(keyframesRule);
            var zeroFrame = keyframesRule.Find("0%");
            Assert.NotNull(zeroFrame);
        }

        [Fact]
        public void Parse_Keyframes_HasHundredPercent()
        {
            var sheet = ParseFixture("AtRules", "001_keyframes.css");
            var keyframesRule = sheet.Rules.OfType<KeyframesRule>().FirstOrDefault();

            Assert.NotNull(keyframesRule);
            var hundredFrame = keyframesRule.Find("100%");
            Assert.NotNull(hundredFrame);
        }

        [Fact]
        public void Parse_Keyframes_ZeroPercentHasTransform()
        {
            var sheet = ParseFixture("AtRules", "001_keyframes.css");
            var keyframesRule = sheet.Rules.OfType<KeyframesRule>().FirstOrDefault();

            Assert.NotNull(keyframesRule);
            var zeroFrame = keyframesRule.Find("0%");
            Assert.NotNull(zeroFrame);

            var transformProp = zeroFrame.Style.GetProperty("transform");
            Assert.NotNull(transformProp);
            Assert.True(transformProp.HasValue);
        }

        [Fact]
        public void Parse_Keyframes_HundredPercentHasTransform()
        {
            var sheet = ParseFixture("AtRules", "001_keyframes.css");
            var keyframesRule = sheet.Rules.OfType<KeyframesRule>().FirstOrDefault();

            Assert.NotNull(keyframesRule);
            var hundredFrame = keyframesRule.Find("100%");
            Assert.NotNull(hundredFrame);

            var transformProp = hundredFrame.Style.GetProperty("transform");
            Assert.NotNull(transformProp);
            Assert.True(transformProp.HasValue);
        }

        [Fact]
        public void Parse_Keyframes_TransformNotRawValue()
        {
            var sheet = ParseFixture("AtRules", "001_keyframes.css");
            var keyframesRule = sheet.Rules.OfType<KeyframesRule>().FirstOrDefault();

            Assert.NotNull(keyframesRule);
            var zeroFrame = keyframesRule.Find("0%");
            Assert.NotNull(zeroFrame);

            var transformProp = zeroFrame.Style.GetProperty("transform");
            Assert.NotNull(transformProp);

            var typedValue = transformProp.TypedValue;
            Assert.NotNull(typedValue);
            Assert.IsNotType<RawValue>(typedValue);
        }
    }
}
