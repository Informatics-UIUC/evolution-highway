using System;
using System.Linq;
using EvolutionHighwayApp.Converters;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.ViewModels
{
    public class CustomTrackViewModel : ModelBase, IDisposable
    {
        #region ViewModel Bindable Properties

        private CustomTrackRegion _customTrack;
        public CustomTrackRegion CustomTrack
        {
            get { return _customTrack; }
            set
            {
                NotifyPropertyChanged(() => CustomTrack, ref _customTrack, value); 
                NotifyPropertyChanged(() => LabelPosition);
            }
        }

        public double? LabelPosition
        {
            get
            {
                if (CustomTrack == null) return null;
                return (double) ScaleConverter.Convert(CustomTrack.MidPoint, null, null, null) - 10;
            }
        }

        public AppSettings AppSettings { get; private set; }

        #endregion

        private static readonly ScaleConverter ScaleConverter = new ScaleConverter();
        private readonly IDisposable _displaySizeChangedObserver;


        public CustomTrackViewModel()
        {
            AppSettings = IoC.Container.Resolve<AppSettings>();

            _displaySizeChangedObserver = IoC.Container.Resolve<IEventPublisher>().GetEvent<DisplaySizeChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e =>
                               {
                                   NotifyPropertyChanged(() => CustomTrack); 
                                   NotifyPropertyChanged(() => LabelPosition);
                               });
        }

        public void Dispose()
        {
            _displaySizeChangedObserver.Dispose();
        }
    }
}
