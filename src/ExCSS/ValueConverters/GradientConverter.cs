using System.Collections.Generic;

namespace ExCSS
{
    using static Converters;

    internal abstract class GradientConverter : IValueConverter
    {
        protected readonly bool IsRepeating;

        protected GradientConverter(bool repeating = false)
        {
            IsRepeating = repeating;
        }

        public IPropertyValue Convert(IEnumerable<Token> value)
        {
            var args = value.ToList();
            var initial = args.Count != 0 ? ConvertFirstArgument(args[0]) : null;
            var offset = initial != null ? 1 : 0;
            var stops = ToGradientStops(args, offset);
            return stops != null ? CreateGradientValue(initial, stops, value) : null;
        }

        public IPropertyValue Construct(Property[] properties)
        {
            return properties.Guard<GradientValue>();
        }

        protected abstract IPropertyValue ConvertFirstArgument(IEnumerable<Token> value);
        protected abstract IGradient CreateGradient(IPropertyValue initial, GradientStop[] stops);

        private IPropertyValue CreateGradientValue(IPropertyValue initial, GradientStop[] stops, IEnumerable<Token> tokens)
        {
            var gradient = CreateGradient(initial, stops);
            return new GradientValue(gradient, initial, stops, tokens);
        }

        private static GradientStop[] ToGradientStops(List<List<Token>> values, int offset)
        {
            var stops = new GradientStop[values.Count - offset];

            for (int i = offset, k = 0; i < values.Count; i++, k++)
            {
                var stop = ToGradientStop(values[i]);
                if (stop == null) return null;
                stops[k] = stop.Value;
            }

            return stops;
        }

        private static GradientStop? ToGradientStop(List<Token> value)
        {
            var color = default(IPropertyValue);
            var position = default(IPropertyValue);
            var items = value.ToItems();

            if (items.Count != 0)
            {
                position = LengthOrPercentConverter.Convert(items[items.Count - 1]);

                if (position != null) items.RemoveAt(items.Count - 1);
            }

            if (items.Count != 0)
            {
                color = ColorConverter.Convert(items[items.Count - 1]);

                if (color != null) items.RemoveAt(items.Count - 1);
            }

            if (items.Count != 0)
                return null;

            var stopColor = ExtractColor(color);
            var stopLocation = ExtractLength(position);

            return new GradientStop(stopColor ?? Color.Transparent, stopLocation ?? Length.Zero);
        }

        private static Color? ExtractColor(IPropertyValue pv)
        {
            if (pv is ITypedPropertyValue typed && typed.GetValue() is Color c)
                return c;
            return null;
        }

        private static Length? ExtractLength(IPropertyValue pv)
        {
            if (pv is ITypedPropertyValue typed)
            {
                var val = typed.GetValue();
                if (val is Length len) return len;
                if (val is Percent pct) return new Length(pct.Value, Length.Unit.Percent);
            }
            return null;
        }

        private sealed class GradientValue : IPropertyValue, ITypedPropertyValue
        {
            private readonly IGradient _gradient;
            private readonly IPropertyValue _initial;
            private readonly GradientStop[] _stops;

            public GradientValue(IGradient gradient, IPropertyValue initial, GradientStop[] stops, IEnumerable<Token> tokens)
            {
                _gradient = gradient;
                _initial = initial;
                _stops = stops;
                Original = new TokenValue(tokens);
            }

            public string CssText
            {
                get
                {
                    var count = _stops.Length;
                    if (_initial != null) count++;
                    var args = new string[count];
                    count = 0;
                    if (_initial != null) args[count++] = _initial.CssText;
                    foreach (var stop in _stops) args[count++] = stop.CssText;
                    return string.Join(", ", args);
                }
            }

            public TokenValue Original { get; }

            public TokenValue ExtractFor(string name) => Original;

            public object GetValue() => _gradient;
        }
    }

    internal sealed class LinearGradientConverter : GradientConverter
    {
        private readonly IValueConverter _converter;

        public LinearGradientConverter(bool repeating = false) : base(repeating)
        {
            _converter = AngleConverter.Or(
                SideOrCornerConverter.StartsWithKeyword(Keywords.To));
        }

        protected override IPropertyValue ConvertFirstArgument(IEnumerable<Token> value)
        {
            return _converter.Convert(value);
        }

        protected override IGradient CreateGradient(IPropertyValue initial, GradientStop[] stops)
        {
            var angle = Angle.Half;
            if (initial is ITypedPropertyValue typed)
            {
                var val = typed.GetValue();
                if (val is Angle a)
                    angle = a;
            }
            return new LinearGradient(angle, stops, IsRepeating);
        }
    }

    internal sealed class RadialGradientConverter : GradientConverter
    {
        private readonly IValueConverter _converter;

        public RadialGradientConverter(bool repeating = false) : base(repeating)
        {
            var position = PointConverter.StartsWithKeyword(Keywords.At).Option(Point.Center);
            var circle = WithOrder(WithAny(Assign(Keywords.Circle, true).Option(true),
                    LengthConverter.Option()),
                position);

            var ellipse = WithOrder(WithAny(Assign(Keywords.Ellipse, false).Option(false),
                    LengthOrPercentConverter.Many(2, 2).Option()),
                position);

            var extents = WithOrder(WithAny(Toggle(Keywords.Circle, Keywords.Ellipse).Option(false),
                Map.RadialGradientSizeModes.ToConverter()), position);

            _converter = circle.Or(ellipse.Or(extents));
        }

        protected override IPropertyValue ConvertFirstArgument(IEnumerable<Token> value)
        {
            return _converter.Convert(value);
        }

        protected override IGradient CreateGradient(IPropertyValue initial, GradientStop[] stops)
        {
            var isCircle = false;
            var position = Point.Center;
            var majorRadius = Length.Zero;
            var minorRadius = Length.Zero;
            var sizeMode = RadialGradient.SizeMode.None;

            if (initial is ITypedPropertyValue typed)
            {
                var val = typed.GetValue();
                if (val is object[] parts)
                {
                    foreach (var part in parts)
                    {
                        if (part is bool b) isCircle = b;
                        else if (part is Point pt) position = pt;
                        else if (part is RadialGradient.SizeMode sm) sizeMode = sm;
                        else if (part is Length len)
                        {
                            if (majorRadius == Length.Zero) majorRadius = len;
                            else minorRadius = len;
                        }
                        else if (part is Length[] lengths && lengths.Length >= 2)
                        {
                            majorRadius = lengths[0];
                            minorRadius = lengths[1];
                        }
                    }
                }
            }

            return new RadialGradient(isCircle, position, majorRadius, minorRadius, sizeMode, stops, IsRepeating);
        }
    }
}
