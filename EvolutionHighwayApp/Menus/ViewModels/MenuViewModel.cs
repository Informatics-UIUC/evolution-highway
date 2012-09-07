using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using EvolutionHighwayApp.Display.ViewModels;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Exceptions;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.Commands;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Repository.Controllers;
using EvolutionHighwayApp.Settings.Views;
using ColorOptionsWindow = EvolutionHighwayApp.Settings.Views.ColorOptionsWindow;

namespace EvolutionHighwayApp.Menus.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public AppSettings AppSettings { get; private set; }

        public int CompGenomeNameFormat
        {
            get { return AppSettings.CompGenomeNameFormat; }
            set { 
                AppSettings.CompGenomeNameFormat ^= value;
                NotifyPropertyChanged(() => CompGenomeNameFormat);
            }
        }

        public Command LoadCustomTrackCommand { get; private set; }
        public Command EditCustomTrackCommand { get; private set; }
        public Command ClearCustomTrackCommand { get; private set; }
        public Command CaptureScreenCommand { get; private set; }
        public Command ResetZoomCommand { get; private set; }
        public Command ViewFullScreenCommand { get; private set; }
        public Command ShowColorOptionsWindowCommand { get; private set; }
        public Command ResetOptionsToDefaultsCommand { get; private set; }

        #endregion

        // This field is needed to ensure that SelectionsController is instantiated before the other classes
        //private readonly DisplayController _selections;

        private readonly IRepositoryController _repositoryController;
        private readonly IEventPublisher _eventPublisher;
        private readonly ColorOptionsWindow _colorOptionsWindow;
        private string _trackData;
        private Delimiter _delimiter;

        public MenuViewModel()
        {
            AppSettings = IoC.Container.Resolve<AppSettings>();

            _repositoryController = IoC.Container.Resolve<IRepositoryController>();
            _eventPublisher = IoC.Container.Resolve<IEventPublisher>();
            _colorOptionsWindow = new ColorOptionsWindow();

            LoadCustomTrackCommand = new Command(LoadCustomTrack, canExecute => true);
            EditCustomTrackCommand = new Command(EditCustomTrack, canExecute => true);
            ClearCustomTrackCommand = new Command(ClearCustomTrack, canExecute => true);
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

            try
            {
                using (var reader = ofd.File.OpenText())
                {
                    _trackData = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK);
                return;
            }

            switch (ofd.File.Extension.ToLower())
            {
                case ".track":
                    _delimiter = new Delimiter {Char = '|', Label = "Vertical Line"};
                    break;

                case ".csv":
                    _delimiter = new Delimiter {Char = ',', Label = "Comma"};
                    break;

                case ".tsv":
                    _delimiter = new Delimiter {Char = '\t', Label = "Tab"};
                    break;

                default:
                    _delimiter = new Delimiter { Char = '\t', Label = "Tab" };
                    break;
            }

            ProcessCustomTrackData(_trackData, _delimiter.Char);
        }

        private void EditCustomTrack(object param)
        {
            Debug.WriteLine("EditCustomTrack invoked");

            var window = new EditCustomTrackWindow(trackData: _trackData, delimiter: _delimiter, resultCallback: (vm, cancelled) =>
            {
                if (cancelled) return;

                _trackData = vm.TrackDataText;
                _delimiter = vm.Delimiter;
                ProcessCustomTrackData(_trackData, _delimiter.Char);
            });

            window.Show();
        }

        private void ClearCustomTrack(object param)
        {
            Debug.WriteLine("ClearCustomTrack invoked");

            _repositoryController.ClearCustomTracks();
            _trackData = null;
            _delimiter = null;
        }

        private void ProcessCustomTrackData(string trackData, char delimiter)
        {
            try
            {
                _repositoryController.AddCustomTrackData(trackData, delimiter);
            }
            catch (ParseErrorException e)
            {
                MessageBox.Show(string.Format("{0}\nLine: {1}\nToken: '{2}'", e.Message, e.LineNumber, e.Text), "Parse error", MessageBoxButton.OK);
                return;
            }

            _eventPublisher.Publish(new CustomTrackDataLoadedEvent());
        }

        private void CaptureScreen(object param)
        {
            Debug.WriteLine("CaptureScreen invoked");
            RefGenomeCollectionViewModel.CaptureScreen();
            _eventPublisher.Publish(new CaptureScreenEvent());
        }

        private void ResetZoom(object param)
        {
            Debug.WriteLine("ResetZoom invoked");
            _eventPublisher.Publish(new ResetZoomEvent());
        }

        private void ViewFullScreen(object param)
        {
            Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
        }
    }
}
