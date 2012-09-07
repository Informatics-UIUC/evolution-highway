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

        private readonly IDisposable _displaySizeChangedObserver;


        public HighlightRegionViewModel()
        {
            AppSettings = IoC.Container.Resolve<AppSettings>();

            _displaySizeChangedObserver = IoC.Container.Resolve<IEventPublisher>().GetEvent<DisplaySizeChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => NotifyPropertyChanged(() => HighlightRegion));
        }

        public override void Dispose()
        {
            base.Dispose();

            _displaySizeChangedObserver.Dispose();
        }
    }
}
