using System;
using System.Linq;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.State;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.ViewModels
{
    public class RefChromosomeCollectionViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public SmartObservableCollection<RefChromosome> RefChromosomes { get; private set; }

        public AppSettings AppSettings { get; set; }

        private RefGenome _refGenome;
        public RefGenome RefGenome
        {
            get { return _refGenome; }
            set
            {
                if (RefGenome == value) return;
                _refGenome = value;

                if (_selections.SelectedRefChromosomes.ContainsKey(_refGenome))
                    RefChromosomes.AddRange(_selections.SelectedRefChromosomes[_refGenome]);

                NotifyPropertyChanged(() => RefGenome);
            }
        }

        #endregion

        private readonly SelectionsController _selections;
        private readonly IDisposable _refChromosomeSelectionChangedObserver;
        
 
        public RefChromosomeCollectionViewModel(SelectionsController selections, IEventPublisher eventPublisher)
            : base(eventPublisher)
        {
            _selections = selections;

            RefChromosomes = new SmartObservableCollection<RefChromosome>();

            _refChromosomeSelectionChangedObserver = EventPublisher.GetEvent<RefChromosomeSelectionDisplayEvent>()
                .Where(e => e.RefGenome == RefGenome)
                .ObserveOnDispatcher()
                .Subscribe(OnRefChromosomeSelectionDisplay);
        }

        private void OnRefChromosomeSelectionDisplay(RefChromosomeSelectionDisplayEvent e)
        {
            var selectedChromosomes = e.SelectedChromosomes.ToList();

            e.RemovedChromosomes.ForEach(c => RefChromosomes.Remove(c));
            e.AddedChromosomes.ForEach(chromosome => RefChromosomes.Insert(selectedChromosomes.IndexOf(chromosome), chromosome));

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
