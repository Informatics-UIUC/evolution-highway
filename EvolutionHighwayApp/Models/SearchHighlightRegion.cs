using System.Windows.Media;

namespace EvolutionHighwayApp.Models
{
    public class SearchHighlightRegion : HighlightRegion
    {
        public override Color Color
        {
            get { return AppSettings.SearchHighlightColor; }
        }

        public SearchHighlightRegion(double start, double end) : base(start, end) { }
    }
}
