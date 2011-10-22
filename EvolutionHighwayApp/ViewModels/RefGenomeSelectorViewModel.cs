using System;
using System.Collections.Generic;
using System.Linq;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.State;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.ViewModels
{
    public class RefGenomeSelectorViewModel : ViewModelBase
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
        private readonly IDisposable _updateSelectionObserver;
        private readonly IDisposable _loadingObserver;
        private readonly IDisposable _refGenomeLoadingObserver;

        public RefGenomeSelectorViewModel(Repository repository, IEventPublisher eventPublisher) 
            : base(eventPublisher)
        {
            _repository = repository;

            Genomes = new SmartObservableCollection<SelectableItem>();

            _dataSourceChangedObserver = EventPublisher.GetEvent<DataSourceChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnDataSourceChanged);

            _updateSelectionObserver = EventPublisher.GetEvent<UpdateSelectionEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnUpdateSelection);

            _loadingObserver = EventPublisher.GetEvent<LoadingEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => IsEnabled = e.IsDoneLoading);

            _refGenomeLoadingObserver = EventPublisher.GetEvent<RefGenomesLoadingEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => IsLoading = !e.IsDoneLoading);
        }

        private void OnDataSourceChanged(DataSourceChangedEvent e)
        {
            _repository.LoadRefGenomes((result, param) =>
                {
                    var items = _repository.RefGenomeMap.Keys.Select(genome =>
                                {
                                    var item = new SelectableItem(genome.Name);
                                    item.PropertyChanged += OnItemPropertyChanged;
                                    return item;
                                }).ToList();

                    items.Sort((a, b) => a.Name.CompareTo(b.Name));
                    Genomes.ForEach(item => item.PropertyChanged -= OnItemPropertyChanged);
                    Genomes.ReplaceWith(items);
                });
        }

        void OnItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals("IsSelected")) return;

            var item = (SelectableItem) sender;
            if (item.IsSelected) 
                OnGenomeSelected(item);
            else 
                OnGenomeUnselected(item);
        }

        private void OnGenomeSelected(SelectableItem item)
        {
            var genomes = _repository.RefGenomeMap.Keys.Where(genome => genome.Name == item.Name);
            OnGenomeSelectionChanged(new List<RefGenome>(genomes), Enumerable.Empty<RefGenome>());
        }

        private void OnGenomeUnselected(SelectableItem item)
        {
            var genomes = _repository.RefGenomeMap.Keys.Where(genome => genome.Name == item.Name);
            OnGenomeSelectionChanged(Enumerable.Empty<RefGenome>(), new List<RefGenome>(genomes));
        }

        public void OnGenomeSelectionReordered()
        {
            OnGenomeSelectionChanged(Enumerable.Empty<RefGenome>(), Enumerable.Empty<RefGenome>());
        }

        private void OnGenomeSelectionChanged(IEnumerable<RefGenome> addedGenomes, IEnumerable<RefGenome> removedGenomes)
        {
            var selectedGenomes = (from item in Genomes
                                  where item.IsSelected
                                  from genome in _repository.RefGenomeMap.Keys
                                  where genome.Name == item.Name
                                  select genome).ToList();
                
            EventPublisher.Publish(new RefGenomeSelectionChangedEvent
            {
                AddedGenomes = addedGenomes,
                RemovedGenomes = removedGenomes,
                SelectedGenomes = selectedGenomes
            });
        }

        private void OnUpdateSelection(UpdateSelectionEvent e)
        {
            var genomeNames = e.SelectedCompGenomes.Select(g => g.RefChromosome.RefGenome.Name).Distinct().ToList();
            Genomes.ForEach(item => item.PropertyChanged -= OnItemPropertyChanged);
            Genomes.ForEach(g => g.IsSelected = genomeNames.Contains(g.Name));
            Genomes.ForEach(item => item.PropertyChanged += OnItemPropertyChanged);
        }

        public override void Dispose()
        {
            base.Dispose();

            _dataSourceChangedObserver.Dispose();
            _updateSelectionObserver.Dispose();
            _loadingObserver.Dispose();
            _refGenomeLoadingObserver.Dispose();

            Genomes.Clear();
        }
    }
}
