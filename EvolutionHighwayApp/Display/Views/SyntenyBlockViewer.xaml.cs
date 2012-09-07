using System.Windows;
using EvolutionHighwayApp.Display.ViewModels;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Display.Views
{
    public partial class SyntenyBlockViewer
    {
        public static readonly DependencyProperty SyntenyRegionProperty =
            DependencyProperty.Register("SyntenyRegion", typeof(SyntenyRegion), typeof(SyntenyBlockViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((SyntenyBlockViewer)o).ViewModel.SyntenyRegion = (SyntenyRegion)e.NewValue)));

        public SyntenyRegion SyntenyRegion
        {
            get { return (SyntenyRegion)GetValue(SyntenyRegionProperty); }
            set { SetValue(SyntenyRegionProperty, value); }
        }

        public SyntenyBlockViewModel ViewModel
        {
            get { return DataContext as SyntenyBlockViewModel; }
        }

        public SyntenyBlockViewer()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = new SyntenyBlockViewModel();
            Unloaded += delegate { ViewModel.Dispose(); };
        }
    }
}
