using System.IO;

namespace ExCSS
{
    public abstract class ChildSelector : StylesheetNode, ISelector
    {
        private readonly string _name;
        public int Step { get; private set; }
        public int Offset { get; private set; }
        protected ISelector Kind;

        protected ChildSelector(string name)
        {
            _name = name;
        }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            string formula;
            if (Step == 0)
            {
                formula = Offset.ToString();
            }
            else
            {
                var a = Step == 1 ? string.Empty : (Step == -1 ? "-" : Step.ToString());
                var b = Offset switch
                {
                    > 0 => "+" + Offset,
                    < 0 => Offset.ToString(),
                    _ => string.Empty
                };
                formula = $"{a}n{b}";
            }

            writer.Write(":{0}({1})", _name, formula);
        }

        public Priority Specificity => Priority.OneClass;
        public string Text => this.ToCss();

        internal ChildSelector With(int step, int offset, ISelector kind)
        {
            Step = step;
            Offset = offset;
            Kind = kind;
            return this;
        }
    }
}