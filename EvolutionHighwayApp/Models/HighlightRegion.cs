using System.Windows.Media;
using EvolutionHighwayApp.Infrastructure;

namespace EvolutionHighwayApp.Models
{
    public abstract class HighlightRegion : Region
    {
        public abstract Color Color { get; }

        protected AppSettings AppSettings { get; private set; }

        protected HighlightRegion(double start, double end) : base(start, end)
        {
            AppSettings = IoC.Container.Resolve<AppSettings>();
        }
    }
}
