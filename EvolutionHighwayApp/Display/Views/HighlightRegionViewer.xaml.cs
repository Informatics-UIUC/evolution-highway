using System.Windows;
using EvolutionHighwayApp.Display.ViewModels;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Display.Views
{
    public partial class HighlightRegionViewer
    {
        public static readonly DependencyProperty HighlightRegionProperty =
            DependencyProperty.Register("HighlightRegion", typeof(HighlightRegion), typeof(HighlightRegionViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((HighlightRegionViewer)o).ViewModel.HighlightRegion = (HighlightRegion)e.NewValue)));

        public HighlightRegion HighlightRegion
        {
            get { return (HighlightRegion)GetValue(HighlightRegionProperty); }
            set { SetValue(HighlightRegionProperty, value); }
        }

        private HighlightRegionViewModel ViewModel
        {
            get { return DataContext as HighlightRegionViewModel; }
        }

        public HighlightRegionViewer()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = new HighlightRegionViewModel();
            Unloaded += delegate { ViewModel.Dispose(); };
        }
    }
}
