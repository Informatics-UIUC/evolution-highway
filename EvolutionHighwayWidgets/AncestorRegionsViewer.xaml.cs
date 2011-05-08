using System;
using System.Windows.Browser;
using System.Windows.Controls;
using EvolutionHighwayModel;

namespace EvolutionHighwayWidgets
{
    public partial class AncestorRegionsViewer : UserControl
    {
        private const string GENOME_BROWSER_URL =
            "http://genome.ucsc.edu/cgi-bin/hgTracks?clade=mammal&org={0}&db={1}&position=chr{2}:{3}-{4}";

        public AncestorRegionsViewer()
        {
            InitializeComponent();
        }

        private void OnSyntenyBlockMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var block = (SyntenyBlock) sender;
            var region = (AncestorRegion) block.DataContext;

            var parts = region.ComparativeSpecies.Chromosome.Genome.Name.Split(':');
            var url = string.Format(GENOME_BROWSER_URL, parts[0], parts[1], region.ComparativeSpecies.Chromosome.Name,
                          region.Start, region.End);
            HtmlPage.Window.Navigate(new Uri(url), "_genomeBrowser", "toolbar=1,menubar=1,location=1,status=1,resizable=1,scrollbars=1");
        }
    }
}
