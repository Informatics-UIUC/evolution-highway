using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace EvolutionHighwayApp
{
    public partial class App : Application
    {
        public App()
        {
            Startup += OnApplicationStartup;
            UnhandledException += OnApplicationUnhandledException;

            InitializeComponent();
        }

        private void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            RootVisual = new MainPage();
            MouseWheelService.Enable(RootVisual);

            foreach (var data in e.InitParams)
                Resources.Add(data.Key, data.Value);
        }

        private static void OnApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // a ChildWindow control.
            if (!Debugger.IsAttached)
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                ChildWindow errorWin = new ErrorWindow(e.ExceptionObject);
                errorWin.Show();
            }
        }
    }
}