using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.State;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.ViewModels
{
    public class CompGenomeSelectorViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public SmartObservableCollection<SelectableItem> Genomes { get; private set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { NotifyPropertyChanged(() => IsLoading, ref _isLoading, value); }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { NotifyPropertyChanged(() => IsEnabled, ref _isEnabled, value); }
        }

        #endregion

        private readonly Repository _repository;
        private readonly IDisposable _dataSourceChangedObserver;
        private readonly IDisposable _refChromosomeSelectionChangedObserver;
        private readonly IDisposable _updateSelectionObserver;
        private readonly IDisposable _loadingObserver;
        private readonly IDisposable _compGenomeLoadingObserver;

        private IEnumerable<RefChromosome> _selectedChromosomes;

        public CompGenomeSelectorViewModel(Repository repository, IEventPublisher eventPublisher) 
            : base(eventPublisher)
        {
            _repository = repository;

            Genomes = new SmartObservableCollection<SelectableItem>();

            _dataSourceChangedObserver = EventPublisher.GetEvent<DataSourceChangedEvent>()
                .Subscribe(e => Genomes.Clear());

            _refChromosomeSelectionChangedObserver = EventPublisher.GetEvent<RefChromosomeSelectionChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnRefChromosomeSelectionChanged);

            _updateSelectionObserver = EventPublisher.GetEvent<UpdateSelectionEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnUpdateSelection);

            _loadingObserver = EventPublisher.GetEvent<LoadingEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => IsEnabled = e.IsDoneLoading);

            _compGenomeLoadingObserver = EventPublisher.GetEvent<CompGenomesLoadingEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => IsLoading = !e.IsDoneLoading);
        }

        private void OnRefChromosomeSelectionChanged(RefChromosomeSelectionChangedEvent e)
        {
            _selectedChromosomes = e.SelectedChromosomes;

            _repository.LoadCompGenomes(e.AddedChromosomes, (result, param) =>
                {
                    var selections = UpdateSelections(e.AddedChromosomes, e.RemovedChromosomes, e.SelectedChromosomes);
                    var addedGenomes = selections.Item1;
                    var removedGenomes = selections.Item2; 
                    var selectedItems = selections.Item3;
              
                    var addedSelectedGenomes = (from item in selectedItems
                                               join genome in addedGenomes on item.Name equals genome.Name
                                               select genome).ToList();
                    var removedSelectedGenomes = (from item in selectedItems
                                                 join genome in removedGenomes on item.Name equals genome.Name
                                                 select genome).ToList();

                    if (!addedSelectedGenomes.IsEmpty() || !removedSelectedGenomes.IsEmpty())
                        OnGenomeSelectionChanged(addedSelectedGenomes, removedSelectedGenomes);
                });
        }

        private Tuple<List<CompGenome>, List<CompGenome>, List<SelectableItem>> 
            UpdateSelections(IEnumerable<RefChromosome> addedChromosomes, IEnumerable<RefChromosome> removedChromosomes, IEnumerable<RefChromosome> selectedChromosomes)
        {
            var addedGenomes = (from chromosome in addedChromosomes
                           from genome in _repository.RefChromosomeMap[chromosome]
                           select genome).ToList();
            var removedGenomes = (from chromosome in removedChromosomes
                             from genome in _repository.RefChromosomeMap[chromosome]
                             select genome).ToList();
            var selectedGenomes = (from chromosome in selectedChromosomes
                                  from genome in _repository.RefChromosomeMap[chromosome]
                                  select genome).ToList();

            var selectedItems = Genomes.Where(item => item.IsSelected).ToList();

            var namesToRemove = removedGenomes.Select(g => g.Name).Distinct().ToList();
            var namesToKeep = selectedGenomes.Select(g => g.Name).Distinct().ToList();
            var itemsToRemove = (from name in namesToRemove.Except(namesToKeep)
                                 join item in Genomes on name equals item.Name
                                 select item).ToList();
            itemsToRemove.ForEach(item => item.PropertyChanged -= OnItemPropertyChanged);
            Genomes.RemoveRange(itemsToRemove);

            var namesToAdd = addedGenomes.Select(g => g.Name).Distinct().ToList();
            namesToAdd.Sort();
            var itemsToAdd = (from name in namesToAdd
                              where !Genomes.Any(item => item.Name == name)
                              select new SelectableItem(name)).ToList();
            itemsToAdd.ForEach(item => item.PropertyChanged += OnItemPropertyChanged);
            Genomes.AddRange(itemsToAdd);

            return new Tuple<List<CompGenome>, List<CompGenome>, List<SelectableItem>>(addedGenomes, removedGenomes, selectedItems);
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals("IsSelected")) return;

            var item = (SelectableItem)sender;
            if (item.IsSelected)
                OnGenomeSelected(item);
            else
                OnGenomeUnselected(item);
        }

        private void OnGenomeSelected(SelectableItem item)
        {
            var genomes = from chromosome in _selectedChromosomes
                          from genome in _repository.RefChromosomeMap[chromosome]
                          where genome.Name == item.Name
                          select genome;

            OnGenomeSelectionChanged(new List<CompGenome>(genomes), Enumerable.Empty<CompGenome>());
        }

        private void OnGenomeUnselected(SelectableItem item)
        {
            var genomes = from chromosome in _selectedChromosomes
                          from genome in _repository.RefChromosomeMap[chromosome]
                          where genome.Name == item.Name
                          select genome;

            OnGenomeSelectionChanged(Enumerable.Empty<CompGenome>(), new List<CompGenome>(genomes));
        }

        public void OnGenomeSelectionReordered()
        {
            OnGenomeSelectionChanged(Enumerable.Empty<CompGenome>(), Enumerable.Empty<CompGenome>());
        }

        private void OnGenomeSelectionChanged(IEnumerable<CompGenome> addedGenomes, IEnumerable<CompGenome> removedGenomes)
        {
            var selectedGenomes = (from item in Genomes
                                  where item.IsSelected
                                  from chromosome in _selectedChromosomes
                                  from genome in _repository.RefChromosomeMap[chromosome]
                                  where genome.Name == item.Name
                                  select genome).ToList();

            EventPublisher.Publish(new CompGenomeSelectionChangedEvent
            {
                AddedGenomes = addedGenomes,
                RemovedGenomes = removedGenomes,
                SelectedGenomes = selectedGenomes
            });
        }

        private void OnUpdateSelection(UpdateSelectionEvent e)
        {
            Genomes.Clear();
            _selectedChromosomes = e.SelectedCompGenomes.Select(g => g.RefChromosome).Distinct().ToList();
            UpdateSelections(_selectedChromosomes, Enumerable.Empty<RefChromosome>(), _selectedChromosomes);

            var selectedCompGenomeNames = e.SelectedCompGenomes.Select(g => g.Name).Distinct().ToList();
            Genomes.ForEach(item => item.PropertyChanged -= OnItemPropertyChanged);
            Genomes.ForEach(g => g.IsSelected = selectedCompGenomeNames.Contains(g.Name));
            Genomes.ForEach(item => item.PropertyChanged += OnItemPropertyChanged);
        }

        public override void Dispose()
        {
            base.Dispose();

            _dataSourceChangedObserver.Dispose();
            _refChromosomeSelectionChangedObserver.Dispose();
            _updateSelectionObserver.Dispose();
            _loadingObserver.Dispose();
            _compGenomeLoadingObserver.Dispose();
            _selectedChromosomes = null;

            Genomes.Clear();
        }
    }
}
