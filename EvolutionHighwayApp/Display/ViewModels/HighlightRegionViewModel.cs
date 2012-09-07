using System;
using System.Linq;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.Display.ViewModels
{
    public class HighlightRegionViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        private HighlightRegion _highlightRegion;
        public HighlightRegion HighlightRegion
        {
            get { return _highlightRegion; }
            set { NotifyPropertyChanged(() => HighlightRegion, ref _highlightRegion, value); }
        }

        public AppSettings AppSettings { get; private set; }

        #endregion

        private readonly IEventPublisher _eventPublisher;

        private readonly IDisposable _displaySizeChangedObserver;
        private readonly IDisposable _highlightRegionColorChangedObserver;

        public HighlightRegionViewModel()
        {
            AppSettings = IoC.Container.Resolve<AppSettings>();

            _eventPublisher = IoC.Container.Resolve<IEventPublisher>();

            _displaySizeChangedObserver = _eventPublisher.GetEvent<DisplaySizeChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => NotifyPropertyChanged(() => HighlightRegion));

            _highlightRegionColorChangedObserver = _eventPublisher.GetEvent<HighlightColorChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => NotifyPropertyChanged(() => HighlightRegion));
        }

        public override void Dispose()
        {
            base.Dispose();

            _displaySizeChangedObserver.Dispose();
            _highlightRegionColorChangedObserver.Dispose();
        }
    }
}
