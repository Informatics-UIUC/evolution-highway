using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EvolutionHighwayApp.Display.Controllers;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.Repository.Controllers;
using EvolutionHighwayApp.Selection.Controllers;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Display.ViewModels
{
    public class RefChromosomeCollectionViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public SmartObservableCollection<RefChromosome> RefChromosomes { get; private set; }

        public AppSettings AppSettings { get; private set; }

        private RefGenome _refGenome;
        public RefGenome RefGenome
        {
            get { return _refGenome; }
            set
            {
                if (_refGenome == value)
                    return;

                _refGenome = value;

                RefChromosomes.ReplaceWith(_displayController.GetVisibleRefChromosomes(_refGenome));
            }
        }

        #endregion

        private readonly IEventPublisher _eventPublisher;
        private readonly IDisplayController _displayController;
        private readonly ISelectionController _selectionController;
        private readonly IRepositoryController _repositoryController;
        private readonly IDisposable _refChromosomeSelectionChangedObserver;
        
 
        public RefChromosomeCollectionViewModel()
        {
            RefChromosomes = new SmartObservableCollection<RefChromosome>();
            AppSettings = IoC.Container.Resolve<AppSettings>();

            _displayController = IoC.Container.Resolve<IDisplayController>();
            _repositoryController = IoC.Container.Resolve<IRepositoryController>();
            _selectionController = IoC.Container.Resolve<ISelectionController>();
            _eventPublisher = IoC.Container.Resolve<IEventPublisher>();

            _refChromosomeSelectionChangedObserver = _eventPublisher.GetEvent<RefChromosomeSelectionDisplayEvent>()
                .Where(e => e.RefGenome == RefGenome)
                //.ObserveOnDispatcher()
                .Subscribe(OnRefChromosomeSelectionDisplay);
        }

        private void OnRefChromosomeSelectionDisplay(RefChromosomeSelectionDisplayEvent e)
        {
            var selectedChromosomes = e.SelectedChromosomes.ToList();

            e.RemovedChromosomes.Except(e.AddedChromosomes).ForEach(c => RefChromosomes.Remove(c));
            e.AddedChromosomes.Except(RefChromosomes).ForEach(chromosome => RefChromosomes.Insert(selectedChromosomes.IndexOf(chromosome), chromosome));

            for (var i = 0; i < selectedChromosomes.Count; i++)
            {
                var chromosome = selectedChromosomes.ElementAt(i);
                if (RefChromosomes.ElementAt(i) == chromosome) continue;

                RefChromosomes.Remove(chromosome);
                RefChromosomes.Insert(i, chromosome);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            _refChromosomeSelectionChangedObserver.Dispose();
            _refGenome = null;
            
            RefChromosomes.Clear();
        }
    }
}
