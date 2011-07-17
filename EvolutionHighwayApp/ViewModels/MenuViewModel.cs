using System.Diagnostics;
using System.Linq;
using System.Windows;
using EvolutionHighwayApp.Converters;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure.Commands;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.State;
using EvolutionHighwayApp.Views;

namespace EvolutionHighwayApp.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public AppSettings AppSettings { get; set; }

        public int CompGenomeNameFormat
        {
            get { return AppSettings.CompGenomeNameFormat; }
            set { 
                AppSettings.CompGenomeNameFormat ^= value;
                NotifyPropertyChanged(() => CompGenomeNameFormat);
            }
        }

        public Command CaptureScreenCommand { get; private set; }
        public Command ResetZoomCommand { get; private set; }
        public Command ViewFullScreenCommand { get; private set; }
        public Command ShowColorOptionsWindowCommand { get; private set; }
        public Command ResetOptionsToDefaultsCommand { get; private set; }

        #endregion

        private SelectionsController _selections;
        private readonly ColorOptionsWindow _colorOptionsWindow;

        public MenuViewModel(IEventPublisher eventPublisher, SelectionsController selections) 
            : base(eventPublisher)
        {
            _selections = selections;
            _colorOptionsWindow = new ColorOptionsWindow();

            CaptureScreenCommand = new Command(CaptureScreen, canExecute => true);
            ResetZoomCommand = new Command(ResetZoom, canExecute => true);
            ViewFullScreenCommand = new Command(ViewFullScreen, canExecute => true);
            ShowColorOptionsWindowCommand = new Command(ShowColorOptionsWindow, canExecute => true);
            ResetOptionsToDefaultsCommand = new Command(ResetOptionsToDefaults, canExecute => true);
        }

        private void ResetOptionsToDefaults(object obj)
        {
            if (MessageBox.Show("Are you sure you want to reset all options to their default values?", 
                                "Reset To Defaults", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                AppSettings.ResetToDefaults();
        }

        private void ShowColorOptionsWindow(object obj)
        {
            _colorOptionsWindow.Show();
        }

        private void CaptureScreen(object param)
        {
            Debug.WriteLine("CaptureScreen invoked");
            RefGenomeCollectionViewModel.CaptureScreen();
            EventPublisher.Publish(new CaptureScreenEvent());
        }

        private void ResetZoom(object param)
        {
            Debug.WriteLine("ResetZoom invoked");
            
            if (_selections.SelectedCompGenomes.Count > 0)
                ScaleConverter.DataMaximum = _selections.SelectedCompGenomes.Keys.Max(c => c.Length);

            AppSettings.BlockWidth = 24d;
            AppSettings.DisplaySize = 500d;
        }

        private void ViewFullScreen(object param)
        {
            Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
        }
    }
}
