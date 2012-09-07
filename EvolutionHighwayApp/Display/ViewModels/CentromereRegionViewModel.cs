using System;
using System.Linq;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.Display.ViewModels
{
    public class CentromereRegionViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        private CentromereRegion _centromereRegion;
        public CentromereRegion CentromereRegion
        {
            get { return _centromereRegion; }
            set { NotifyPropertyChanged(() => CentromereRegion, ref _centromereRegion, value); }
        }

        public AppSettings AppSettings { get; private set; }

        #endregion

        private readonly IDisposable _displaySizeChangedObserver;


        public CentromereRegionViewModel()
        {
            AppSettings = IoC.Container.Resolve<AppSettings>();

            _displaySizeChangedObserver = IoC.Container.Resolve<IEventPublisher>().GetEvent<DisplaySizeChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => NotifyPropertyChanged(() => CentromereRegion));
        }

        public override void Dispose()
        {
            base.Dispose();

            _displaySizeChangedObserver.Dispose();
        }
    }
}
