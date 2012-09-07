using System.Windows.Media;

namespace EvolutionHighwayApp.Models
{
    public class BreakpointClassificationHighlightRegion : HighlightRegion
    {
        public override Color Color
        {
            get { return AppSettings.BreakpointClassificationHighlightColor; }
        }

        public BreakpointClassificationHighlightRegion(double start, double end) : base(start, end) { }
    }
}
