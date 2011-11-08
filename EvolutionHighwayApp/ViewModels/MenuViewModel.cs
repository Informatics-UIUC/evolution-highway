using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Exceptions;
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

        public Command LoadCustomTrackCommand { get; private set; }
        public Command PasteCustomTrackCommand { get; private set; }
        public Command CaptureScreenCommand { get; private set; }
        public Command ResetZoomCommand { get; private set; }
        public Command ViewFullScreenCommand { get; private set; }
        public Command ShowColorOptionsWindowCommand { get; private set; }
        public Command ResetOptionsToDefaultsCommand { get; private set; }

        #endregion

        // This field is needed to ensure that SelectionsController is instantiated before the other classes
        private readonly SelectionsController _selections;

        private readonly Repository _repository;
        private readonly ColorOptionsWindow _colorOptionsWindow;

        public MenuViewModel(IEventPublisher eventPublisher, SelectionsController selections, Repository repository) 
            : base(eventPublisher)
        {
            _selections = selections;
            _repository = repository;
            _colorOptionsWindow = new ColorOptionsWindow();

            LoadCustomTrackCommand = new Command(LoadCustomTrack, canExecute => true);
            PasteCustomTrackCommand = new Command(PasteCustomTrack, canExecute => true);
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

        private void LoadCustomTrack(object param)
        {
            Debug.WriteLine("LoadCustomTrack invoked");
            var ofd = new OpenFileDialog
                          {
                              Filter = "Track files (*.track;*.csv;*.tsv)|*.track;*.csv;*.tsv|All files (*.*)|*.*"
                          };
            if (ofd.ShowDialog() != true) return;

            string trackData;
            try
            {
                using (var reader = ofd.File.OpenText())
                {
                    trackData = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK);
                return;
            }

            char delimiter;
            switch (ofd.File.Extension.ToLower())
            {
                case ".track":
                    delimiter = '|';
                    break;

                case ".csv":
                    delimiter = ',';
                    break;

                case ".tsv":
                    delimiter = '\t';
                    break;

                default:
                    delimiter = '\t';
                    break;
            }

            ProcessCustomTrackData(trackData, delimiter);
        }

        private void PasteCustomTrack(object param)
        {
            Debug.WriteLine("PasteCustomTrack invoked");

            var window = new PasteCustomTrackWindow((vm, cancelled) =>
            {
                if (cancelled) return;
                
                ProcessCustomTrackData(vm.TrackDataText, vm.Delimiter.Char);
            });

            window.Show();
        }

        private void ProcessCustomTrackData(string trackData, char delimiter)
        {
            try
            {
                _repository.AddCustomTrackData(trackData, delimiter);
            }
            catch (ParseErrorException e)
            {
                MessageBox.Show(string.Format("{0}\nLine: {1}\nToken: '{2}'", e.Message, e.LineNumber, e.Text), "Parse error", MessageBoxButton.OK);
                return;
            }

            EventPublisher.Publish(new CustomTrackDataLoadedEvent());
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
            EventPublisher.Publish(new ResetZoomEvent());
        }

        private void ViewFullScreen(object param)
        {
            Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
        }
    }
}
