using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    public sealed class BackgroundExpander : IShorthandExpander
    {
        private static readonly string[] Names =
        {
            PropertyNames.BackgroundColor,
            PropertyNames.BackgroundImage,
            PropertyNames.BackgroundPosition,
            PropertyNames.BackgroundSize,
            PropertyNames.BackgroundRepeat,
            PropertyNames.BackgroundAttachment,
            PropertyNames.BackgroundOrigin,
            PropertyNames.BackgroundClip
        };

        private static readonly HashSet<string> RepeatKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "repeat", "no-repeat", "space", "round", "repeat-x", "repeat-y"
        };

        private static readonly HashSet<string> AttachmentKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "scroll", "fixed", "local"
        };

        private static readonly HashSet<string> BoxKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "border-box", "padding-box", "content-box"
        };

        public IReadOnlyList<string> ShorthandNames => new[] { PropertyNames.Background };
        public IReadOnlyList<string> LonghandNames => Names;

        public IReadOnlyDictionary<string, IStyleValue> Expand(IStyleValue value)
        {
            var result = new Dictionary<string, IStyleValue>(StringComparer.OrdinalIgnoreCase);

            if (value == null)
                return result;

            // Handle global keywords
            if (value is KeywordValue kw && IsGlobalKeyword(kw.Value))
            {
                foreach (var name in Names)
                    result[name] = value;
                return result;
            }

            // Split into layers (comma-separated)
            IReadOnlyList<IStyleValue> layers = value is StyleValueList list ? list : new[] { value };
            
            var bgImage = new List<IStyleValue>();
            var bgPosition = new List<IStyleValue>();
            var bgSize = new List<IStyleValue>();
            var bgRepeat = new List<IStyleValue>();
            var bgAttachment = new List<IStyleValue>();
            var bgOrigin = new List<IStyleValue>();
            var bgClip = new List<IStyleValue>();
            IStyleValue bgColor = null;

            for (int i = 0; i < layers.Count; i++)
            {
                var layer = layers[i];
                var tokens = ExpanderHelpers.ExtractValues(layer);
                
                // Defaults for this layer
                IStyleValue img = new KeywordValue("none");
                IStyleValue pos = new StyleValueTuple(new KeywordValue("0%"), new KeywordValue("0%"));
                IStyleValue size = new KeywordValue("auto");
                IStyleValue repeat = new KeywordValue("repeat");
                IStyleValue attach = new KeywordValue("scroll");
                IStyleValue origin = new KeywordValue("padding-box");
                IStyleValue clip = new KeywordValue("border-box");
                
                // Parsing state
                bool foundSize = false;
                bool foundOrigin = false;
                bool foundClip = false;
                var positionTokens = new List<IStyleValue>();
                var sizeTokens = new List<IStyleValue>();

                foreach (var token in tokens)
                {
                    if (token is KeywordValue k)
                    {
                        var kv = k.Value;
                        
                        // 1. Repeat
                        if (RepeatKeywords.Contains(kv))
                        {
                            if (string.Equals(kv, "repeat-x", StringComparison.OrdinalIgnoreCase))
                                repeat = new StyleValueTuple(new KeywordValue("repeat"), new KeywordValue("no-repeat"));
                            else if (string.Equals(kv, "repeat-y", StringComparison.OrdinalIgnoreCase))
                                repeat = new StyleValueTuple(new KeywordValue("no-repeat"), new KeywordValue("repeat"));
                            else
                                repeat = k;
                            continue;
                        }

                        // 2. Attachment
                        if (AttachmentKeywords.Contains(kv))
                        {
                            attach = k;
                            continue;
                        }

                        // 3. Box (Origin / Clip)
                        if (BoxKeywords.Contains(kv))
                        {
                            if (!foundOrigin)
                            {
                                origin = k;
                                foundOrigin = true;
                            }
                            else
                            {
                                clip = k;
                                foundClip = true;
                            }
                            continue;
                        }

                        // 4. None (Image)
                        if (string.Equals(kv, "none", StringComparison.OrdinalIgnoreCase))
                        {
                            img = k;
                            continue;
                        }
                    }

                    // 5. Size separator '/'
                    if (token.CssText == "/") // Delimiter check
                    {
                        foundSize = true;     // Start collecting size
                        continue;
                    }

                    // 6. Color (Last layer only)
                    if (i == layers.Count - 1 && IsColor(token))
                    {
                        bgColor = token;
                        continue;
                    }

                    // 7. Image (Url or Gradient)
                    if (IsImage(token))
                    {
                        img = token;
                        continue;
                    }

                    // 8. Position or Size
                    if (foundSize)
                    {
                        sizeTokens.Add(token);
                    }
                    else
                    {
                        // Assume position if it looks like length/percent/keyword
                        // or if we haven't found anything else.
                        // Simple heuristic: if it's a number/length/percent/position-keyword, add to position
                        positionTokens.Add(token);
                    }
                }

                // Finalize layer values
                if (positionTokens.Count > 0)
                {
                    pos = positionTokens.Count == 1 ? positionTokens[0] : new StyleValueTuple(positionTokens);
                }
                
                if (sizeTokens.Count > 0)
                {
                    size = sizeTokens.Count == 1 ? sizeTokens[0] : new StyleValueTuple(sizeTokens);
                }

                if (foundOrigin && !foundClip)
                {
                    clip = origin; // If one box value is present, it sets both
                }

                bgImage.Add(img);
                bgPosition.Add(pos);
                bgSize.Add(size);
                bgRepeat.Add(repeat);
                bgAttachment.Add(attach);
                bgOrigin.Add(origin);
                bgClip.Add(clip);
            }

            // Assign results
            result[PropertyNames.BackgroundImage] = CreateListOrSingle(bgImage);
            result[PropertyNames.BackgroundPosition] = CreateListOrSingle(bgPosition);
            result[PropertyNames.BackgroundSize] = CreateListOrSingle(bgSize);
            result[PropertyNames.BackgroundRepeat] = CreateListOrSingle(bgRepeat);
            result[PropertyNames.BackgroundAttachment] = CreateListOrSingle(bgAttachment);
            result[PropertyNames.BackgroundOrigin] = CreateListOrSingle(bgOrigin);
            result[PropertyNames.BackgroundClip] = CreateListOrSingle(bgClip);
            result[PropertyNames.BackgroundColor] = bgColor ?? new Color(0, 0, 0, 0); // Transparent default

            return result;
        }

        private static IStyleValue CreateListOrSingle(List<IStyleValue> list)
        {
            if (list.Count == 1) return list[0];
            return new StyleValueList(list);
        }

        private static bool IsGlobalKeyword(string keyword)
        {
            return string.Equals(keyword, "inherit", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(keyword, "initial", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(keyword, "unset", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(keyword, "revert", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsColor(IStyleValue value)
        {
            return value is Color;
        }

        private static bool IsImage(IStyleValue value)
        {
            return value is UrlValue || value is IGradient; 
            // Note: GradientFunctionValue handling depends on typed pipeline, 
            // usually it implements IImageSource or similar if available.
            // For now, assume UrlValue or check for function type 'linear-gradient' etc if typed value is generic FunctionValue.
            // But let's check basic types.
        }
    }
}
