using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using EvolutionHighwayApp.Pages;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp
{
    public partial class MainPage
    {
        public string Version
        {
            get
            {
                var asm = Assembly.GetExecutingAssembly();
                if (asm.FullName == null) return "";

                var version = new AssemblyName(asm.FullName).Version;
                return string.Format("v{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            }
        }

        public MainPage()
        {
            InitializeComponent();

            DataContext = this;

            if (Application.Current.InstallState == InstallState.Installed)
            {
                Application.Current.CheckAndDownloadUpdateCompleted += OnApplicationUpdateCompleted;
                Application.Current.CheckAndDownloadUpdateAsync();
            }
        }

        private static void OnApplicationUpdateCompleted(object sender, CheckAndDownloadUpdateCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                new ErrorWindow(e.Error).Show();
                return;
            }

            if (!e.UpdateAvailable) return;

            MessageBox.Show("An update to this application has been downloaded and installed." + Environment.NewLine +
                            "Please restart this application to start working with the new version.",
                            "Application Update", MessageBoxButton.OK);
        }

        // After the Frame navigates, ensure the HyperlinkButton representing the current page is selected
        private void OnContentFrameNavigated(object sender, NavigationEventArgs e)
        {
            LinksStackPanel.Children.Select(child => child as HyperlinkButton)
                .Where(hb => hb != null && hb.NavigateUri != null)
                .ForEach(hb => VisualStateManager.GoToState(hb, 
                    hb.NavigateUri.ToString().Equals(e.Uri.ToString()) ? "ActiveLink" : "InactiveLink", true));
        }

        // If an error occurs during navigation, show an error window
        private void OnContentFrameNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ChildWindow errorWin = new ErrorWindow(e.Uri);
            errorWin.Show();
        }
    }
}