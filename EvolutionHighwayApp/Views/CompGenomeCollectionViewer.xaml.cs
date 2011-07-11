using System.Windows;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
{
    public partial class CompGenomeCollectionViewer
    {
        public static readonly DependencyProperty RefChromosomeProperty =
            DependencyProperty.Register("RefChromosome", typeof(RefChromosome), typeof(CompGenomeCollectionViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((CompGenomeCollectionViewer)o).ViewModel.RefChromosome = (RefChromosome)e.NewValue)));

        public static readonly DependencyProperty BlockWidthProperty =
            DependencyProperty.Register("BlockWidth", typeof(int), typeof(CompGenomeCollectionViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((CompGenomeCollectionViewer)o).ViewModel.BlockWidth = (int)e.NewValue)));

        public RefChromosome RefChromosome
        {
            get { return (RefChromosome)GetValue(RefChromosomeProperty); }
            set { SetValue(RefChromosomeProperty, value); }
        }

        public int BlockWidth
        {
            get { return (int) GetValue(BlockWidthProperty); }
            set { SetValue(BlockWidthProperty, value);}
        }

        private CompGenomeCollectionViewModel ViewModel
        {
            get { return DataContext as CompGenomeCollectionViewModel; }
        }

        public CompGenomeCollectionViewer()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = IoC.Container.Resolve<CompGenomeCollectionViewModel>();
            Unloaded += delegate { ViewModel.Dispose(); IoC.Container.Release(ViewModel); };
        }
    }
}
