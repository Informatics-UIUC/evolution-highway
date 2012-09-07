using System;
using System.Linq;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.Display.ViewModels
{
    public class HeterochromatinRegionCollectionViewModel : ViewModelBase
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
        private readonly IDisposable _heterochromatinRegionDisplayEventObserver;

        public HeterochromatinRegionCollectionViewModel()
        {
            _eventPublisher = IoC.Container.Resolve<IEventPublisher>();

            _heterochromatinRegionDisplayEventObserver = _eventPublisher.GetEvent<HeterochromatinRegionDisplayEvent>()
                .Where(e => e.Chromosome == RefChromosome)
                .ObserveOnDispatcher()
                .Subscribe(OnHeterochromatinRegionDisplay);
        }

        private void OnHeterochromatinRegionDisplay(HeterochromatinRegionDisplayEvent e)
        {
            NotifyPropertyChanged(() => RefChromosome);
        }

        public override void Dispose()
        {
            base.Dispose();

            _heterochromatinRegionDisplayEventObserver.Dispose();
        }
    }
}
