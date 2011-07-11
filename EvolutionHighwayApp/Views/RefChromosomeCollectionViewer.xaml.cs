using System.Windows;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
{
    public partial class RefChromosomeCollectionViewer
    {
        public static readonly DependencyProperty RefGenomeProperty =
            DependencyProperty.Register("RefGenome", typeof(RefGenome), typeof(RefChromosomeCollectionViewer), new PropertyMetadata(
                new PropertyChangedCallback((o, e) => ((RefChromosomeCollectionViewer)o).ViewModel.RefGenome = ((RefGenome)e.NewValue))));

        public RefGenome RefGenome
        {
            get { return (RefGenome)GetValue(RefGenomeProperty); }
            set { SetValue(RefGenomeProperty, value); }
        }

        private RefChromosomeCollectionViewModel ViewModel
        {
            get { return DataContext as RefChromosomeCollectionViewModel; }
        }

        public RefChromosomeCollectionViewer()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = IoC.Container.Resolve<RefChromosomeCollectionViewModel>();
            Unloaded += delegate { ViewModel.Dispose(); IoC.Container.Release(ViewModel); };
        }
    }
}
