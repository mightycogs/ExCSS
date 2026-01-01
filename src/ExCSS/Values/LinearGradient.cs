using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExCSS
{
    public sealed class LinearGradient : IGradient, IStyleValue
    {
        public LinearGradient(Angle angle, GradientStop[] stops, bool repeating = false)
        {
            _stops = stops;
            Angle = angle;
            IsRepeating = repeating;
        }

        private readonly GradientStop[] _stops;

        public Angle Angle { get; }
        public IEnumerable<GradientStop> Stops => _stops.AsEnumerable();
        public bool IsRepeating { get; }

        public string CssText
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(IsRepeating ? "repeating-linear-gradient(" : "linear-gradient(");

                if (Angle != Angle.Half)
                {
                    sb.Append(Angle.CssText);
                    if (_stops.Length > 0)
                        sb.Append(", ");
                }

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
    }
}
