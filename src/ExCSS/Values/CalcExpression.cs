namespace ExCSS
{
    public interface ICalcExpression
    {
        string CssText { get; }
    }

    public sealed class CalcBinaryExpression : ICalcExpression
    {
        public ICalcExpression Left { get; }
        public CalcOperator Operator { get; }
        public ICalcExpression Right { get; }

        public CalcBinaryExpression(ICalcExpression left, CalcOperator op, ICalcExpression right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public string CssText => $"{Left.CssText} {OperatorText} {Right.CssText}";

        private string OperatorText => Operator switch
        {
            CalcOperator.Add => "+",
            CalcOperator.Subtract => "-",
            CalcOperator.Multiply => "*",
            CalcOperator.Divide => "/",
            _ => "?"
        };
    }

    public enum CalcOperator
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    public sealed class CalcLiteralExpression : ICalcExpression
    {
        public IStyleValue Value { get; }

        public CalcLiteralExpression(IStyleValue value)
        {
            Value = value;
        }

        public string CssText => Value.CssText;
    }

    public sealed class CalcGroupExpression : ICalcExpression
    {
        public ICalcExpression Inner { get; }

        public CalcGroupExpression(ICalcExpression inner)
        {
            Inner = inner;
        }

        public string CssText => $"({Inner.CssText})";
    }

    public sealed class CalcVarExpression : ICalcExpression
    {
        public VarValue Variable { get; }

        public CalcVarExpression(VarValue variable)
        {
            Variable = variable;
        }

        public string CssText => Variable.CssText;
    }
}
