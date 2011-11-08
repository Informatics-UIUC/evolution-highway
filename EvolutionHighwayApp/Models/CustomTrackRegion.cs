using System.Windows.Media;

namespace EvolutionHighwayApp.Models
{
    public class CustomTrackRegion : Region
    {
        public string Genome { get; private set; }
        public string Chromosome { get; private set; }
        public string Label { get; private set; }
        public Color? Color { get; private set; }

        public CustomTrackRegion(string genome, string chromosome, double start, double end, string label, Color? color = null) : base(start, end)
        {
            Genome = genome;
            Chromosome = chromosome;
            Label = label;
            Color = color;
        }

        public override string ToString()
        {
            return string.Format("{0} genome: {1} chromosome: {2} label: {3} color: {4}", base.ToString(), Genome, Chromosome, Label, Color);
        }
    }
}
