using System;
using System.Windows;
using System.Windows.Controls;

namespace EvolutionHighwayApp.Pages
{
    public partial class ErrorWindow
    {
        public ErrorWindow(Exception e)
        {
            InitializeComponent();

            if (e != null)
                ErrorTextBox.Text = e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace;
        }

        public ErrorWindow(Uri uri)
        {
            InitializeComponent();
            
            if (uri != null)
                ErrorTextBox.Text = string.Format("Page not found: \"{0}\"", uri);
        }

        public ErrorWindow(string message, string details)
        {
            InitializeComponent();
            ErrorTextBox.Text = message + Environment.NewLine + Environment.NewLine + details;
        }

        private void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}