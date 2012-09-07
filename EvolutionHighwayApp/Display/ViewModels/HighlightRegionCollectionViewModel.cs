using System;
using System.Collections.Generic;
using System.Linq;
using EvolutionHighwayApp.Display.Controllers;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;

namespace EvolutionHighwayApp.Display.ViewModels
{
    public class HighlightRegionCollectionViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        private RefChromosome _refChromosome;
        public RefChromosome RefChromosome
        {
            get { return _refChromosome; }
            set
            {
                NotifyPropertyChanged(() => RefChromosome, ref _refChromosome, value);
                HighlightRegions = _displayController.GetHighlightRegions(_refChromosome);
            }
        }

        private IEnumerable<Region> _highlightRegions;
        public IEnumerable<Region> HighlightRegions
        {
            get { return _highlightRegions; }
            private set { NotifyPropertyChanged(() => HighlightRegions, ref _highlightRegions, value); }
        }

        #endregion

        private readonly IDisplayController _displayController;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDisposable _highlightRegionDisplayEventObserver;

        public HighlightRegionCollectionViewModel()
        {
            _highlightRegions = new HighlightRegion[0];

            _displayController = IoC.Container.Resolve<IDisplayController>();
            _eventPublisher = IoC.Container.Resolve<IEventPublisher>();

            _highlightRegionDisplayEventObserver = _eventPublisher.GetEvent<HighlightRegionDisplayEvent>()
                .Where(e => e.Chromosome == _refChromosome)
                .ObserveOnDispatcher()
                .Subscribe(OnHighlightRegionDisplay);
        }

        private void OnHighlightRegionDisplay(HighlightRegionDisplayEvent e)
        {
            HighlightRegions = e.Regions;
        }

        public override void Dispose()
        {
            base.Dispose();

            _highlightRegionDisplayEventObserver.Dispose();
        }
    }
}
