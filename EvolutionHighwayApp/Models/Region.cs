using EvolutionHighwayApp.Infrastructure.MVVM;

namespace EvolutionHighwayApp.Models
{
    public abstract class Region : ModelBase
    {
        public double Start { get; private set; }
        public double End { get; private set; }

        public double Span
        {
            get { return End - Start; }
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
