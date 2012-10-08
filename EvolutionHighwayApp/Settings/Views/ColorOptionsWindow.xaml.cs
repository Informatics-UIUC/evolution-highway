using System.Windows.Media;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Settings.ViewModels;

namespace EvolutionHighwayApp.Settings.Views
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

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            IoC.Container.Resolve<AppSettings>().HeterochromatinBgColor = Color.FromArgb(100, 200, 30, 30);
        }
    }
}

