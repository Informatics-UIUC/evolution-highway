using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using EvolutionHighwayApp.Events;
using EvolutionHighwayApp.Infrastructure;
using EvolutionHighwayApp.Infrastructure.Commands;
using EvolutionHighwayApp.Infrastructure.EventBus;
using EvolutionHighwayApp.Infrastructure.MVVM;
using EvolutionHighwayApp.Repository.Controllers;
using EvolutionHighwayApp.Selection.Controllers;
using EvolutionHighwayApp.Selection.Models;
using EvolutionHighwayApp.Utils;

namespace EvolutionHighwayApp.Selection.ViewModels
{
    public class RefChromosomeSelectorViewModel : ViewModelBase
    {
        #region ViewModel Bindable Properties

        public SmartObservableCollection<RefChromosomeItem> Chromosomes { get; private set; }

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

        public ICommand SelectAllCommand { get; private set; }
        #endregion

        private static readonly ChromosomeNameComparer ChromosomeNameComparer = new ChromosomeNameComparer();

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepositoryController _repositoryController;
        private readonly ISelectionController _selectionController;

        private readonly IDisposable _dataSourceChangedObserver;
        private readonly IDisposable _refGenomeSelectionChangedObserver;
        private readonly IDisposable _refChromosomeSelectionChangedObserver;
        private readonly IDisposable _loadingObserver;
        private readonly IDisposable _refChromosomeLoadingObserver;

        public RefChromosomeSelectorViewModel()
        {
            IsEnabled = true;
            Chromosomes = new SmartObservableCollection<RefChromosomeItem>();

            _eventPublisher = IoC.Container.Resolve<IEventPublisher>();
            _repositoryController = IoC.Container.Resolve<IRepositoryController>();
            _selectionController = IoC.Container.Resolve<ISelectionController>();

            _dataSourceChangedObserver = _eventPublisher.GetEvent<DataSourceChangedEvent>()
                .Subscribe(e => { Chromosomes.ForEach(c => c.PropertyChanged -= OnItemPropertyChanged); Chromosomes.Clear(); });

            _refGenomeSelectionChangedObserver = _eventPublisher.GetEvent<RefGenomeSelectionChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnRefGenomeSelectionChanged);

            _refChromosomeSelectionChangedObserver = _eventPublisher.GetEvent<RefChromosomeSelectionChangedEvent>()
                .ObserveOnDispatcher()
                .Subscribe(OnRefChromosomeSelectionChanged);

//            _loadingObserver = EventPublisher.GetEvent<LoadingEvent>()
//                .ObserveOnDispatcher()
//                .Subscribe(e => IsEnabled = e.IsDoneLoading);
//
//            _refChromosomeLoadingObserver = EventPublisher.GetEvent<RefChromosomesLoadingEvent>()
//                .ObserveOnDispatcher()
//                .Subscribe(e => IsLoading = !e.IsDoneLoading);

            SelectAllCommand = new Command(SelectAll, canExecute => true);
        }

        private void OnRefGenomeSelectionChanged(RefGenomeSelectionChangedEvent e)
        {
            _repositoryController.GetRefChromosomes(e.SelectedGenomes.ToList(),
                success =>
                {
                    // TODO: selection logic needs fixing - should not resort for each event - need to maintain user ordering
                    var previouslySelectedChrNames = Chromosomes.Where(c => c.IsSelected).Select(c => c.Name).ToList();
                    var chrNames = success.Result.Select(c => c.Name).Distinct().ToList();
                    chrNames.Sort((a, b) => ChromosomeNameComparer.Compare(a, b));
                    var items = chrNames.Select(
                        name =>
                        {
                            var item = new RefChromosomeItem(name);
                            if (previouslySelectedChrNames.Contains(name))
                                item.IsSelected = true;
                            item.PropertyChanged += OnItemPropertyChanged;
                            return item;
                        });

                    Chromosomes.ForEach(c => c.PropertyChanged -= OnItemPropertyChanged);
                    Chromosomes.ReplaceWith(items);
                });

            // TODO: decide whether to use the beforeLoadCallback in above
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals("IsSelected")) return;

            var item = (RefChromosomeItem)sender;
            if (item.IsSelected)
                OnChromosomeSelected(item);
            else
                OnChromosomeUnselected(item);
        }

        private void OnChromosomeSelected(RefChromosomeItem chromosome)
        {
           _selectionController.SelectRefChromosomesByName(Chromosomes.Where(c => c.IsSelected).Select(c => c.Name).ToList());
        }

        private void OnChromosomeUnselected(RefChromosomeItem chromosome)
        {
            _selectionController.SelectRefChromosomesByName(Chromosomes.Where(c => c.IsSelected).Select(c => c.Name).ToList());  
        }

        public void OnChromosomeSelectionReordered()
        {
            _selectionController.SelectRefChromosomesByName(Chromosomes.Where(c => c.IsSelected).Select(c => c.Name).ToList());
        }

        private void SelectAll(object param)
        {
            var selectAll = bool.Parse(param.ToString());

            var selected = selectAll ? Chromosomes.Select(c => c.Name) : Enumerable.Empty<string>();
            _selectionController.SelectRefChromosomesByName(selected.ToArray());
        }

        private void OnRefChromosomeSelectionChanged(RefChromosomeSelectionChangedEvent e)
        {
            var selectedChromosomeNames = e.SelectedChromosomes.Select(c => c.Name).Distinct().ToList();
            Chromosomes.ForEach(item =>
                                    {
                                        item.PropertyChanged -= OnItemPropertyChanged;
                                        item.IsSelected = selectedChromosomeNames.Contains(item.Name);
                                        item.PropertyChanged += OnItemPropertyChanged;
                                    });
        }

        public override void Dispose()
        {
            base.Dispose();

            _dataSourceChangedObserver.Dispose();
            _refGenomeSelectionChangedObserver.Dispose();
            _refChromosomeSelectionChangedObserver.Dispose();
//            _loadingObserver.Dispose();
//            _refChromosomeLoadingObserver.Dispose();

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
