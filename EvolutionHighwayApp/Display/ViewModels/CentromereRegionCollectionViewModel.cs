using System;
using System.Linq;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.Display.ViewModels
{
    public class CentromereRegionCollectionViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        private RefChromosome _refChromosome;
        public RefChromosome RefChromosome
        {
            get { return _refChromosome; }
            set { NotifyPropertyChanged(() => RefChromosome, ref _refChromosome, value); }
        }

        #endregion

        private readonly IEventPublisher _eventPublisher;
        private readonly IDisposable _centromereRegionDisplayEventObserver;

        public CentromereRegionCollectionViewModel()
        {
            _eventPublisher = IoC.Container.Resolve<IEventPublisher>();

            _centromereRegionDisplayEventObserver = _eventPublisher.GetEvent<CentromereRegionDisplayEvent>()
                .Where(e => e.Chromosome == RefChromosome)
                .ObserveOnDispatcher()
                .Subscribe(OnCentromereRegionDisplay);
        }

        private void OnCentromereRegionDisplay(CentromereRegionDisplayEvent e)
        {
            NotifyPropertyChanged(() => RefChromosome);
        }

        public override void Dispose()
        {
            base.Dispose();

            _centromereRegionDisplayEventObserver.Dispose();
        }
    }
}
