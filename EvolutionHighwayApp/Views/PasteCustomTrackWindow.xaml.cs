using System;
using System.Windows;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Utils;
using EvolutionHighwayApp.ViewModels;

namespace EvolutionHighwayApp.Views
{
    public partial class PasteCustomTrackWindow
    {
        private PasteCustomTrackWindowViewModel ViewModel
        {
            get { return DataContext as PasteCustomTrackWindowViewModel; }
        }

        public PasteCustomTrackWindow(Action<PasteCustomTrackWindowViewModel,bool> resultCallback)
        {
            InitializeComponent();

            if (this.InDesignMode()) return;

            DataContext = IoC.Container.Resolve<PasteCustomTrackWindowViewModel>(new { resultCallback = resultCallback });
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

