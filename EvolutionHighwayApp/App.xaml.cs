using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EvolutionHighwayApp.Pages;
using EvolutionHighwayApp.Utils;

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
            LoadInitParams(e.InitParams);
            AppSetup.Setup();

            RootVisual = new MainPage();
        }

        private void LoadInitParams(IDictionary<String, String> initParams)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;

            // When running outside the browser, retrieve initParams from isolated storage.
            if (Application.Current.IsRunningOutOfBrowser)
            {
                initParams = new Dictionary<string, string>();
                settings.Where(s => s.Key.StartsWith("initParam_")).ForEach(s => initParams.Add(s.Key.Substring(10), s.Value.ToString()));
            }
                // Otherwise, save initParams to isolated storage.
            else
                initParams.ForEach(p => settings.Set("initParam_" + p.Key, p.Value));

            initParams.ForEach(p => Resources.Add(p.Key, p.Value));
        }

        private static void OnApplicationUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // a ChildWindow control.
            if (!System.Diagnostics.Debugger.IsAttached)
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