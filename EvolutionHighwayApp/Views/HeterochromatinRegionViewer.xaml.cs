using System.Windows;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
{
    public partial class HeterochromatinRegionViewer
    {
        public static readonly DependencyProperty HeterochromatinRegionProperty =
            DependencyProperty.Register("HeterochromatinRegion", typeof(HeterochromatinRegion), typeof(HeterochromatinRegionViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((HeterochromatinRegionViewer)o).ViewModel.HeterochromatinRegion = (HeterochromatinRegion)e.NewValue)));

        public HeterochromatinRegion HeterochromatinRegion
        {
            get { return (HeterochromatinRegion)GetValue(HeterochromatinRegionProperty); }
            set { SetValue(HeterochromatinRegionProperty, value); }
        }

        private HeterochromatinRegionViewModel ViewModel
        {
            get { return DataContext as HeterochromatinRegionViewModel; }
        }

        public HeterochromatinRegionViewer()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = new HeterochromatinRegionViewModel();
            Unloaded += delegate { ViewModel.Dispose(); };
        }
    }
}
