using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
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
            IDictionary<String, String> initParams;

            // When running outside the browser, retrieve initParams from isolated storage.
            if (Application.Current.IsRunningOutOfBrowser)
            {
                using (var file = IsolatedStorageFile.GetUserStoreForApplication())
                using (var stream = new IsolatedStorageFileStream("initParams.txt", FileMode.Open, file))
                {
                    // The serializer requires a reference to System.Runtime.Serialization.dll.
                    var serializer = new DataContractSerializer(typeof(Dictionary<String, String>));
                    initParams = (Dictionary<String, String>)serializer.ReadObject(stream);
                }
            }
            // Otherwise, save initParams to isolated storage.
            else
            {
                initParams = e.InitParams;

                using (var file = IsolatedStorageFile.GetUserStoreForApplication())
                using (var stream = new IsolatedStorageFileStream("initParams.txt", FileMode.Create, file))
                {
                    var serializer = new DataContractSerializer(typeof(Dictionary<string, string>));
                    serializer.WriteObject(stream, initParams);
                }
            }

            RootVisual = new MainPage();
            MouseWheelService.Enable(RootVisual);

            foreach (var data in initParams)
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