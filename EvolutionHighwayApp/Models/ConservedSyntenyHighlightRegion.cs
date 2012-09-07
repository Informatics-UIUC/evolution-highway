using System.Windows.Media;

namespace EvolutionHighwayApp.Models
{
    public class ConservedSyntenyHighlightRegion : HighlightRegion
    {
        public override Color Color
        {
            get { return AppSettings.ConservedSyntenyHighlightColor; }
        }

        public ConservedSyntenyHighlightRegion(double start, double end) : base(start, end) { }
    }
}
