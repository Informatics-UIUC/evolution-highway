using System;
using System.Linq;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.Display.ViewModels
{
    public class HeterochromatinRegionViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        private HeterochromatinRegion _heterochromatinRegion;
        public HeterochromatinRegion HeterochromatinRegion
        {
            get { return _heterochromatinRegion; }
            set { NotifyPropertyChanged(() => HeterochromatinRegion, ref _heterochromatinRegion, value); }
        }

        public AppSettings AppSettings { get; private set; }

        #endregion

        private readonly IDisposable _displaySizeChangedObserver;


        public HeterochromatinRegionViewModel()
        {
            _displaySizeChangedObserver = IoC.Container.Resolve<IEventPublisher>().GetEvent<DisplaySizeChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => NotifyPropertyChanged(() => HeterochromatinRegion));

            AppSettings = IoC.Container.Resolve<AppSettings>();
        }

        public override void Dispose()
        {
            base.Dispose();

            _displaySizeChangedObserver.Dispose();
        }
    }
}
