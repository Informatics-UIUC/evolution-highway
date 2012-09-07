using System.Windows;
using EvolutionHighwayApp.Display.ViewModels;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Display.Views
{
    public partial class HeterochromatinRegionCollectionViewer
    {
        public static readonly DependencyProperty RefChromosomeProperty =
            DependencyProperty.Register("RefChromosome", typeof(RefChromosome), typeof(HeterochromatinRegionCollectionViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((HeterochromatinRegionCollectionViewer)o).ViewModel.RefChromosome = (RefChromosome)e.NewValue)));

        public RefChromosome RefChromosome
        {
            get { return (RefChromosome)GetValue(RefChromosomeProperty); }
            set { SetValue(RefChromosomeProperty, value); }
        }

        private HeterochromatinRegionCollectionViewModel ViewModel
        {
            get { return DataContext as HeterochromatinRegionCollectionViewModel; }
        }

        public HeterochromatinRegionCollectionViewer()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = new HeterochromatinRegionCollectionViewModel();
            Unloaded += delegate { ViewModel.Dispose(); };
        }
    }
}
