using System;
using System.Collections.Generic;
using System.Linq;

namespace ExCSS
{
    /// <summary>
    /// Computed style with expanded shorthands and resolved values.
    /// Designed for consumption by rendering engines.
    /// </summary>
    public sealed class ComputedStyle
    {
        private readonly Dictionary<string, IStyleValue> _properties =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _prefixes =
            new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Set a property value, automatically expanding shorthands.
        /// </summary>
        public void SetProperty(string name, IStyleValue value)
        {
            if (string.IsNullOrEmpty(name) || value == null) return;

            if (ShorthandRegistry.IsShorthand(name))
            {
                var expanded = ShorthandRegistry.Expand(name, value);
                foreach (var kvp in expanded)
                {
                    _properties[kvp.Key] = kvp.Value;
                    UpdatePrefixes(kvp.Key);
                }
            }
            else
            {
                _properties[name] = value;
                UpdatePrefixes(name);
            }
        }

        /// <summary>
        /// Get a property value.
        /// </summary>
        public IStyleValue GetProperty(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            return _properties.TryGetValue(name, out var v) ? v : null;
        }

        /// <summary>
        /// Get a property value as specific type.
        /// </summary>
        public T GetProperty<T>(string name) where T : class, IStyleValue
        {
            return GetProperty(name) as T;
        }

        /// <summary>
        /// Try to get a property value as specific type.
        /// </summary>
        public bool TryGetProperty<T>(string name, out T value) where T : IStyleValue
        {
            var prop = GetProperty(name);
            if (prop is T typed)
            {
                value = typed;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Check if any property with given prefix exists.
        /// </summary>
        public bool HasAnyWithPrefix(string prefix) =>
            !string.IsNullOrEmpty(prefix) && _prefixes.Contains(prefix);

        // Semantic queries
        public bool HasBorder => HasAnyWithPrefix("border-width") || HasAnyWithPrefix("border-color");
        public bool HasBorderRadius => HasAnyWithPrefix("border-radius") ||
                                       _properties.ContainsKey("border-top-left-radius");
        public bool HasBoxShadow => _properties.ContainsKey("box-shadow");
        public bool HasBackground => _properties.ContainsKey("background-color") ||
                                     _properties.ContainsKey("background-image");
        public bool HasTransform => _properties.ContainsKey("transform");
        public bool HasMargin => HasAnyWithPrefix("margin");
        public bool HasPadding => HasAnyWithPrefix("padding");

        /// <summary>
        /// Get all properties.
        /// </summary>
        public IEnumerable<KeyValuePair<string, IStyleValue>> GetAllProperties() => _properties;

        /// <summary>
        /// Number of properties set.
        /// </summary>
        public int Count => _properties.Count;

        private void UpdatePrefixes(string name)
        {
            if (string.IsNullOrEmpty(name)) return;
            var parts = name.Split('-');
            var prefix = string.Empty;
            foreach (var part in parts)
            {
                prefix = string.IsNullOrEmpty(prefix) ? part : $"{prefix}-{part}";
                _prefixes.Add(prefix);
            }
        }
    }
}
