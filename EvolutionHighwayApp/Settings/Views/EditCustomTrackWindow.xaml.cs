using System;
using System.Windows;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Settings.ViewModels;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Settings.Views
{
    public partial class EditCustomTrackWindow
    {
        private EditCustomTrackWindowViewModel ViewModel
        {
            get { return DataContext as EditCustomTrackWindowViewModel; }
        }

        public EditCustomTrackWindow(Action<EditCustomTrackWindowViewModel,bool> resultCallback, 
            string trackData = null, Delimiter delimiter = null)
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = new EditCustomTrackWindowViewModel(resultCallback)
                              {TrackDataText = trackData, Delimiter = delimiter};
            Unloaded += delegate { ViewModel.Dispose(); };
        }

        private void OnOKButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

