using System.Windows.Controls;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
{
    public partial class DataSourceSelector
    {
        private DataSourceSelectorViewModel ViewModel
        {
            get { return DataContext as DataSourceSelectorViewModel; }
        }

        public DataSourceSelector()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = IoC.Container.Resolve<DataSourceSelectorViewModel>();
            Unloaded += delegate { ViewModel.Dispose(); IoC.Container.Release(ViewModel); };
        }

        private void OnDataSourceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.DataSourceSelectionChanged(((ComboBox) sender).SelectedValue.ToString());
        }
    }
}
