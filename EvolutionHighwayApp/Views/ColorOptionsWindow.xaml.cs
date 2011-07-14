using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
{
    public partial class ColorOptionsWindow
    {
        private ColorOptionsWindowViewModel ViewModel
        {
            get { return DataContext as ColorOptionsWindowViewModel; }
        }

        public ColorOptionsWindow()
        {
            InitializeComponent();

            DataContext = new ColorOptionsWindowViewModel();
            Unloaded += delegate { ViewModel.Dispose(); };
        }

        private void OnOKButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

