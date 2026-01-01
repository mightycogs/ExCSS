// ReSharper disable UnusedMember.Global

namespace ExCSS
{
    public struct Point
    {
        /// <summary>
        ///     Gets the (50%, 50%) point.
        /// </summary>
        public static readonly Point Center = new(Length.Half, Length.Half);

        /// <summary>
        ///     Gets the (0, 0) point.
        /// </summary>
        public static readonly Point LeftTop = new(Length.Zero, Length.Zero);

        /// <summary>
        ///     Gets the (100%, 0) point.
        /// </summary>
        public static readonly Point RightTop = new(Length.Full, Length.Zero);

        /// <summary>
        ///     Gets the (100%, 100%) point.
        /// </summary>
        public static readonly Point RightBottom = new(Length.Full, Length.Full);

        /// <summary>
        ///     Gets the (0, 100%) point.
        /// </summary>
        public static readonly Point LeftBottom = new(Length.Zero, Length.Full);

        public Point(Length x, Length y)
        {
            X = x;
            Y = y;
        }

        public Length X { get; }
        public Length Y { get; }

        public string CssText => $"{X.CssText} {Y.CssText}";

        public override string ToString() => CssText;

        public static bool operator ==(Point a, Point b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Point a, Point b) => !(a == b);

        public override bool Equals(object obj) => obj is Point p && this == p;
        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode();
    }
}
