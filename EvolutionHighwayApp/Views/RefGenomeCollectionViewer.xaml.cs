using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
{
    public partial class RefGenomeCollectionViewer
    {
        private RefGenomeCollectionViewModel ViewModel
        {
            get { return DataContext as RefGenomeCollectionViewModel; }
        }

        public RefGenomeCollectionViewer()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = IoC.Container.Resolve<RefGenomeCollectionViewModel>(new { viewer = this });
            Unloaded += delegate { ViewModel.Dispose(); IoC.Container.Release(ViewModel); };
        }
    }
}
