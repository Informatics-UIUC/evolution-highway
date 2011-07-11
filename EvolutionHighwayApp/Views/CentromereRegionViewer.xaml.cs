using System.Windows;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
{
    public partial class CentromereRegionViewer
    {
        public static readonly DependencyProperty CentromereRegionProperty =
            DependencyProperty.Register("CentromereRegion", typeof(CentromereRegion), typeof(CentromereRegionViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((CentromereRegionViewer)o).ViewModel.CentromereRegion = (CentromereRegion)e.NewValue)));

        public CentromereRegion CentromereRegion
        {
            get { return (CentromereRegion)GetValue(CentromereRegionProperty); }
            set { SetValue(CentromereRegionProperty, value); }
        }

        private CentromereRegionViewModel ViewModel
        {
            get { return DataContext as CentromereRegionViewModel; }
        }

        public CentromereRegionViewer()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = new CentromereRegionViewModel();
            Unloaded += delegate { ViewModel.Dispose(); };
        }
    }
}
