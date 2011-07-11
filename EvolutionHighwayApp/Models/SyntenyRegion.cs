namespace EvolutionHighwayApp.Models
{
    public class SyntenyRegion : Region
    {
        public string Chromosome { get; private set; }
        public string Label { get; private set; }

        public int? Sign { get; private set; }
        public double? ModStart { get; private set; }
        public double? ModEnd { get; private set; }

        public double? ModSpan
        {
            get { return (ModStart.HasValue && ModEnd.HasValue) ? ModEnd - ModStart : null; }
        }

        public CompGenome CompGenome { get; private set; }

        public SyntenyRegion(double start, double end, string chromosome, string label, CompGenome compGenome,
            double? modStart = null, double? modEnd = null, int? sign = null) : base(start, end)
        {
            Chromosome = chromosome;
            Label = label;
            CompGenome = compGenome;
            ModStart = modStart;
            ModEnd = modEnd;
            Sign = sign;
        }
    }
}
