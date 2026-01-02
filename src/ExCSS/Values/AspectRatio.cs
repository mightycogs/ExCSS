using System;
using System.Globalization;

namespace ExCSS
{
    /// <summary>
    /// Represents a CSS aspect-ratio value (e.g., 16/9, 1.5, auto).
    /// </summary>
    public struct AspectRatio : IEquatable<AspectRatio>, IStyleValue, IPrimitiveValue
    {
        /// <summary>
        /// Gets an auto aspect-ratio value.
        /// </summary>
        public static readonly AspectRatio Auto = new(0f, 0f, true);

        /// <summary>
        /// Gets a square (1/1) aspect-ratio value.
        /// </summary>
        public static readonly AspectRatio Square = new(1f, 1f);

        public AspectRatio(float width, float height, bool isAuto = false)
        {
            Width = width;
            Height = height;
            IsAuto = isAuto;
        }

        public AspectRatio(float ratio)
        {
            Width = ratio;
            Height = 1f;
            IsAuto = false;
        }

        /// <summary>
        /// The width component of the ratio.
        /// </summary>
        public float Width { get; }

        /// <summary>
        /// The height component of the ratio.
        /// </summary>
        public float Height { get; }

        /// <summary>
        /// Whether this is the 'auto' value.
        /// </summary>
        public bool IsAuto { get; }

        /// <summary>
        /// Gets the computed ratio value (width / height).
        /// Returns 0 for auto.
        /// </summary>
        public float Value => IsAuto || Height == 0 ? 0f : Width / Height;

        public string CssText
        {
            get
            {
                if (IsAuto) return "auto";
                if (Height == 1f) return Width.ToString(CultureInfo.InvariantCulture);
                return $"{Width.ToString(CultureInfo.InvariantCulture)} / {Height.ToString(CultureInfo.InvariantCulture)}";
            }
        }

        StyleValueType IStyleValue.Type => StyleValueType.AspectRatio;

        public bool Equals(AspectRatio other)
        {
            return IsAuto == other.IsAuto && Width == other.Width && Height == other.Height;
        }

        public static bool operator ==(AspectRatio a, AspectRatio b) => a.Equals(b);
        public static bool operator !=(AspectRatio a, AspectRatio b) => !a.Equals(b);

        public override bool Equals(object obj)
        {
            if (obj is AspectRatio other) return Equals(other);
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Width.GetHashCode();
                hash = hash * 31 + Height.GetHashCode();
                hash = hash * 31 + IsAuto.GetHashCode();
                return hash;
            }
        }

        public override string ToString() => CssText;
    }
}
