using System;
using System.Windows;
using System.Windows.Browser;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
{
    public partial class SyntenyBlockCollectionViewer
    {
        public static readonly DependencyProperty CompGenomeProperty =
            DependencyProperty.Register("CompGenome", typeof(CompGenome), typeof(SyntenyBlockCollectionViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((SyntenyBlockCollectionViewer)o).ViewModel.CompGenome = (CompGenome)e.NewValue)));

        public CompGenome CompGenome
        {
            get { return (CompGenome)GetValue(CompGenomeProperty); }
            set { SetValue(CompGenomeProperty, value); }
        }

        private SyntenyBlockCollectionViewModel ViewModel
        {
            get { return DataContext as SyntenyBlockCollectionViewModel; }
        }

        public SyntenyBlockCollectionViewer()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = new SyntenyBlockCollectionViewModel();
        }

        private const string GENOME_BROWSER_URL =
            "http://genome.ucsc.edu/cgi-bin/hgTracks?clade=mammal&org={0}&db={1}&position=chr{2}:{3}-{4}";

        private void OnMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewer = (SyntenyBlockViewer) sender;
            var region = viewer.ViewModel.SyntenyRegion;

            var parts = region.CompGenome.RefChromosome.RefGenome.Name.Split(':');
            var url = string.Format(GENOME_BROWSER_URL, parts[0], parts[1], region.CompGenome.RefChromosome.Name, region.Start, region.End);
            HtmlPage.Window.Navigate(new Uri(url), "_genomeBrowser", "toolbar=1,menubar=1,location=1,status=1,resizable=1,scrollbars=1");
        }
    }
}
