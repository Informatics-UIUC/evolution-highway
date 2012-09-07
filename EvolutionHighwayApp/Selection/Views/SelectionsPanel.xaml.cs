using EvolutionHighwayApp.Selection.ViewModels;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Selection.Views
{
    public partial class SelectionsPanel
    {
        public SelectionsPanelViewModel ViewModel
        {
            get { return DataContext as SelectionsPanelViewModel; }
        }

        public SelectionsPanel()
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = new SelectionsPanelViewModel(this);
            Unloaded += delegate { ViewModel.Dispose(); };
        }

        public void ExpandSelections()
        {
            _accordion.SelectAll();
        }
    }
}
