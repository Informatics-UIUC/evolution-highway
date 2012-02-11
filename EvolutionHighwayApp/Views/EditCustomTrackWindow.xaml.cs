using System;
using System.Windows;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
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

            var vm = IoC.Container.Resolve<EditCustomTrackWindowViewModel>(new { resultCallback = resultCallback });
            vm.TrackDataText = trackData;
            vm.Delimiter = delimiter;

            DataContext = vm;
            Unloaded += delegate { ViewModel.Dispose(); IoC.Container.Release(ViewModel); };
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

