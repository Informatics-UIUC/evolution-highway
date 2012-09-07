using System.Windows.Controls;
using EvolutionHighwayApp.Selection.ViewModels;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Selection.Views
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

            DataContext = new DataSourceSelectorViewModel();
            Unloaded += delegate { ViewModel.Dispose(); };
        }

        private void OnDataSourceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.DataSourceSelectionChanged(((ComboBox) sender).SelectedValue.ToString());
        }
    }
}
