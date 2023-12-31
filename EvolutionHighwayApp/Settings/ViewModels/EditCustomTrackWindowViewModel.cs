using System;
using EvolutionHighwayApp.Infrastructure.Commands;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.Settings.ViewModels
{
    public class EditCustomTrackWindowViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        private string _trackDataText;
        public string TrackDataText
        {
            get { return _trackDataText; }
            set
            {
                NotifyPropertyChanged(() => TrackDataText, ref _trackDataText, value);
                SubmitCommand.UpdateCanExecute();
            }
        }

        private Delimiter _delimiter;
        public Delimiter Delimiter
        {
            get { return _delimiter; }
            set
            {
                NotifyPropertyChanged(() => Delimiter, ref _delimiter, value);
                SubmitCommand.UpdateCanExecute();
            }
        }

        public Command SubmitCommand { get; private set; }
        public Command CancelCommand { get; private set; }

        #endregion

        public EditCustomTrackWindowViewModel(Action<EditCustomTrackWindowViewModel, bool> resultCallback)
        {
            SubmitCommand = new Command(o => resultCallback(this, false), canExecute => !string.IsNullOrWhiteSpace(TrackDataText) && Delimiter != null);
            CancelCommand = new Command(o => resultCallback(this, true), canExecute => true);
        }
    }
}
