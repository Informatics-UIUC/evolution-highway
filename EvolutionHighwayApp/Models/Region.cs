namespace EvolutionHighwayApp.Models
{
    public abstract class Region
    {
        public double Start { get; private set; }
        public double End { get; private set; }

        public double Span
        {
            get { return End - Start; }
        }

        public double MidPoint
        {
            get { return Start + Span/2; }
        }

        protected Region(double start, double end)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return string.Format("start: {0} end: {1} span: {2}", Start, End, Span);
        }
    }
}
