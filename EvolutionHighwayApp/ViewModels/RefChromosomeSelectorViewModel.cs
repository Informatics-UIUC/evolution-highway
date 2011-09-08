using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Models;
using EvolutionHighwayApp.State;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.ViewModels
{
    public class RefChromosomeSelectorViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public SmartObservableCollection<SelectableItem> Chromosomes { get; private set; }

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

        private static readonly ChromosomeNameComparer ChromosomeNameComparer = new ChromosomeNameComparer();

        private readonly Repository _repository;
        private readonly IDisposable _dataSourceChangedObserver;
        private readonly IDisposable _refGenomeSelectionChangedObserver;
        private readonly IDisposable _loadingObserver;
        private readonly IDisposable _refChromosomeLoadingObserver;

        private IEnumerable<RefGenome> _selectedGenomes;

        public RefChromosomeSelectorViewModel(Repository repository, IEventPublisher eventPublisher) 
            : base(eventPublisher)
        {
            _repository = repository;

            Chromosomes = new SmartObservableCollection<SelectableItem>();

            _dataSourceChangedObserver = EventPublisher.GetEvent<DataSourceChangedEvent>()
                .Subscribe(e => Chromosomes.Clear());

            _refGenomeSelectionChangedObserver = EventPublisher.GetEvent<RefGenomeSelectionChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnRefGenomeSelectionChanged);

            _loadingObserver = EventPublisher.GetEvent<LoadingEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => IsEnabled = e.IsDoneLoading);

            _refChromosomeLoadingObserver = EventPublisher.GetEvent<RefChromosomesLoadingEvent>()
                .ObserveOnDispatcher()
                .Subscribe(e => IsLoading = !e.IsDoneLoading);
        }

        private void OnRefGenomeSelectionChanged(RefGenomeSelectionChangedEvent e)
        {
            _selectedGenomes = e.SelectedGenomes;

            _repository.LoadRefChromosomes(e.AddedGenomes, (result, param) =>
                {
                    var selectedGenomes = e.SelectedGenomes;
                    var addedChromosomes = from genome in e.AddedGenomes
                                           from chromosome in _repository.RefGenomeMap[genome]
                                           select chromosome;
                    var removedChromosomes = from genome in e.RemovedGenomes
                                             from chromosome in _repository.RefGenomeMap[genome]
                                             select chromosome;
                    var selectedChromosomes = from genome in selectedGenomes
                                              from chromosome in _repository.RefGenomeMap[genome]
                                              select chromosome;

                    var selectedItems = Chromosomes.Where(item => item.IsSelected).ToList();

                    var namesToRemove = removedChromosomes.Select(c => c.Name).Distinct().ToList();
                    var namesToKeep = selectedChromosomes.Select(c => c.Name).Distinct().ToList();
                    var itemsToRemove = (from name in namesToRemove.Except(namesToKeep)
                                        join item in Chromosomes on name equals item.Name
                                        select item).ToList();
                    itemsToRemove.ForEach(item => item.PropertyChanged -= OnItemPropertyChanged);
                    Chromosomes.RemoveRange(itemsToRemove);

                    var namesToAdd = addedChromosomes.Select(c => c.Name).Distinct().ToList();
                    namesToAdd.Sort((a, b) => ChromosomeNameComparer.Compare(a, b));
                    var itemsToAdd = (from name in namesToAdd
                                     where !Chromosomes.Any(item => item.Name == name)
                                     select new SelectableItem(name)).ToList();
                    itemsToAdd.ForEach(item => item.PropertyChanged += OnItemPropertyChanged);
                    Chromosomes.AddRange(itemsToAdd);

                    var addedSelectedChromosomes = (from item in selectedItems
                                                   join chromosome in addedChromosomes on item.Name equals chromosome.Name
                                                   select chromosome).ToList();
                    var removedSelectedChromosomes = (from item in selectedItems
                                                     join chromosome in removedChromosomes on item.Name equals chromosome.Name
                                                     select chromosome).ToList();
                    
                    if (!addedSelectedChromosomes.IsEmpty() || !removedSelectedChromosomes.IsEmpty())
                        OnChromosomeSelectionChanged(addedSelectedChromosomes, removedSelectedChromosomes);
                });
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals("IsSelected")) return;

            var item = (SelectableItem)sender;
            if (item.IsSelected)
                OnChromosomeSelected(item);
            else
                OnChromosomeUnselected(item);
        }

        private void OnChromosomeSelected(SelectableItem item)
        {
            var chromosomes = from genome in _selectedGenomes
                              from chromosome in _repository.RefGenomeMap[genome]
                              where chromosome.Name == item.Name
                              select chromosome;

            OnChromosomeSelectionChanged(new List<RefChromosome>(chromosomes), Enumerable.Empty<RefChromosome>());
        }

        private void OnChromosomeUnselected(SelectableItem item)
        {
            var chromosomes = from genome in _selectedGenomes
                              from chromosome in _repository.RefGenomeMap[genome]
                              where chromosome.Name == item.Name
                              select chromosome;

            OnChromosomeSelectionChanged(Enumerable.Empty<RefChromosome>(), new List<RefChromosome>(chromosomes));
        }

        public void OnChromosomeSelectionReordered()
        {
            OnChromosomeSelectionChanged(Enumerable.Empty<RefChromosome>(), Enumerable.Empty<RefChromosome>());
        }

        private void OnChromosomeSelectionChanged(IEnumerable<RefChromosome> addedChromosomes, IEnumerable<RefChromosome> removedChromosomes)
        {
            var selectedChromosomes = (from item in Chromosomes
                                      where item.IsSelected
                                      from genome in _selectedGenomes
                                      from chromosome in _repository.RefGenomeMap[genome]
                                      where chromosome.Name == item.Name
                                      select chromosome).ToList();

            EventPublisher.Publish(new RefChromosomeSelectionChangedEvent
            {
                AddedChromosomes = addedChromosomes,
                RemovedChromosomes = removedChromosomes,
                SelectedChromosomes = selectedChromosomes
            });
        }

        public override void Dispose()
        {
            base.Dispose();

            _dataSourceChangedObserver.Dispose();
            _refGenomeSelectionChangedObserver.Dispose();
            _loadingObserver.Dispose();
            _refChromosomeLoadingObserver.Dispose();
            _selectedGenomes = null;

            Chromosomes.Clear();
        }
    }

    internal class ChromosomeNameComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x.Contains("_")) x = x.Substring(0, x.IndexOf("_"));
            if (y.Contains("_")) y = y.Substring(0, y.IndexOf("_"));

            var digits = new Regex("^(\\d+)");

            var match = digits.Match(x);
            var nx = match.Success ? int.Parse(match.Value) : int.MaxValue;
            var sx = match.Success ? x.Substring(match.Length) : x;

            match = digits.Match(y);
            var ny = match.Success ? int.Parse(match.Value) : int.MaxValue;
            var sy = match.Success ? y.Substring(match.Length) : y;

            if (nx != int.MaxValue && ny != int.MaxValue)
                return nx != ny ? nx.CompareTo(ny) : sx.CompareTo(sy);

            return x.CompareTo(y);
        }
    }
}
