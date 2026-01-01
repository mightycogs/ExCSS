using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExCSS
{
    public sealed class RadialGradient : IGradient, IStyleValue
    {
        public enum SizeMode : byte
        {
            None,
            ClosestCorner,
            ClosestSide,
            FarthestCorner,
            FarthestSide
        }

        public RadialGradient(bool circle, Point pt, Length width, Length height, SizeMode sizeMode,
            GradientStop[] stops, bool repeating = false)
        {
            _stops = stops;
            Position = pt;
            MajorRadius = width;
            MinorRadius = height;
            IsRepeating = repeating;
            IsCircle = circle;
            Mode = sizeMode;
        }

        private readonly GradientStop[] _stops;

        public bool IsCircle { get; }

        public SizeMode Mode { get; }
        public Point Position { get; }
        public Length MajorRadius { get; }
        public Length MinorRadius { get; }
        public IEnumerable<GradientStop> Stops => _stops.AsEnumerable();
        public bool IsRepeating { get; }

        public string CssText
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(IsRepeating ? "repeating-radial-gradient(" : "radial-gradient(");

                var hasShape = false;

                if (IsCircle)
                {
                    sb.Append("circle");
                    hasShape = true;
                }
                else if (Mode != SizeMode.None || MajorRadius != Length.Zero || MinorRadius != Length.Zero)
                {
                    sb.Append("ellipse");
                    hasShape = true;
                }

                if (Mode != SizeMode.None)
                {
                    if (hasShape) sb.Append(' ');
                    sb.Append(GetSizeModeText(Mode));
                    hasShape = true;
                }
                else if (MajorRadius != Length.Zero || MinorRadius != Length.Zero)
                {
                    if (hasShape) sb.Append(' ');
                    if (IsCircle)
                    {
                        sb.Append(MajorRadius.CssText);
                    }
                    else
                    {
                        sb.Append(MajorRadius.CssText);
                        sb.Append(' ');
                        sb.Append(MinorRadius.CssText);
                    }
                    hasShape = true;
                }

                if (Position != Point.Center)
                {
                    if (hasShape) sb.Append(' ');
                    sb.Append("at ");
                    sb.Append(Position.CssText);
                    hasShape = true;
                }

                if (hasShape && _stops.Length > 0)
                    sb.Append(", ");

                for (var i = 0; i < _stops.Length; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append(_stops[i].CssText);
                }

                sb.Append(')');
                return sb.ToString();
            }
        }

        public StyleValueType Type => StyleValueType.Gradient;

        public override string ToString() => CssText;

        private static string GetSizeModeText(SizeMode mode)
        {
            return mode switch
            {
                SizeMode.ClosestCorner => "closest-corner",
                SizeMode.ClosestSide => "closest-side",
                SizeMode.FarthestCorner => "farthest-corner",
                SizeMode.FarthestSide => "farthest-side",
                _ => string.Empty
            };
        }
    }
}
